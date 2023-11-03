using System.Net.Sockets;
using RuneRebirth2005.ClientManagement;
using RuneRebirth2005.Entities;
using RuneRebirth2005.Network;
using Serilog;

namespace RuneRebirth2005.PlayerManagement;

public static class PlayerManager
{
    public static Player InitializeClient(TcpClient tcpClient)
    {
        var player = new Player
        {
            Socket = tcpClient,
            NetworkStream = tcpClient.GetStream(),
            Reader = new RSStream(new byte[ServerConfig.BUFFER_SIZE]),
            Writer = new RSStream(new byte[ServerConfig.BUFFER_SIZE])
        };

        return player;
    }

    public static void DisconnectClient(Client client)
    {
        if (client.Index != -1)
            Server.Players[client.Index] = null;

        client.Socket.Close();
        Log.Information($"Client {client.Index} disconnected.");
    }

    public static void AssignAvailablePlayerSlot(Player player)
    {
        for (var i = 1; i < Server.Players.Length; i++)
            if (Server.Players[i].Index == -1)
            {
                player.Index = i;
                Log.Information($"Incoming connection has been assigned index: {player.Index}!");
                return;
            }

        Log.Warning($"Server is full! Disconnecting {player.Socket.Client.RemoteEndPoint}.");
        DisconnectClient(player);
        throw new Exception("Server is full!");
    }

    public static void RegisterPlayer(Player player)
    {
        Server.Players[player.Index] = player;
        Log.Information($"+ [{player.Index}] has been registered!");
    }
}