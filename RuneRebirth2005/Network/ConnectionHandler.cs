using System.Net;
using System.Net.Sockets;
using RuneRebirth2005.PlayerManagement;
using Serilog;

namespace RuneRebirth2005.Network;

public static class ConnectionHandler
{
    private const int MaxClientsPerCycle = 10;
    private static TcpListener tcpListener;

    public static void Initialize()
    {
        tcpListener = new TcpListener(IPAddress.Any, ServerConfig.PORT);
        tcpListener.Start(10);
        Log.Information($"Server started on port {ServerConfig.PORT}");
    }

    public static void AcceptClients()
    {
        for (var i = 0; i < MaxClientsPerCycle; i++)
        {
            if (!tcpListener.Pending())
                continue;

            var tcpClient = tcpListener.AcceptTcpClient();
            
            try
            {
                var player = PlayerManager.InitializeClient(tcpClient);

                if (player.PlayerSession.Socket.Available < 2)
                {
                    PlayerManager.SilentDisconnectPlayer(player);
                    return;
                }

                Log.Information($"Incoming Connection From: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}");
                
                if (player.LoginHandler.Handshake())
                {
                    PlayerManager.AssignAvailablePlayerSlot(player);

                    // int count = Server.Players.Where(x => x?.Index != -1).Count();
                    // player.Data.Location.X = 3200 + count;
                    // player.Data.Location.Y = 3200;

                    PlayerManager.Login(player);
                }
                else
                {
                    PlayerManager.DisconnectPlayer(player);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
            }
        }
    }
}