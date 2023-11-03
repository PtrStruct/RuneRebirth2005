﻿using System.Net;
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
    }

    public static void AcceptClients()
    {
        for (var i = 0; i < MaxClientsPerCycle; i++)
        {
            if (!tcpListener.Pending())
                continue;

            var tcpClient = tcpListener.AcceptTcpClient();
            Log.Information(
                $"+ Incoming Connection From: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString()}");
            try
            {
                var player = PlayerManager.InitializeClient(tcpClient);

                if (player.Socket.Available < 2)
                {
                    PlayerManager.DisconnectClient(player);
                    return;
                }

                if (player.LoginHandler.Login())
                {
                    PlayerManager.AssignAvailablePlayerSlot(player);
                    PlayerManager.RegisterPlayer(player);
                }
                else
                {
                    PlayerManager.DisconnectClient(player);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
            }
        }
    }
}