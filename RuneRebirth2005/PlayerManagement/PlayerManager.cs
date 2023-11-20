using System.Net.Sockets;
using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;
using RuneRebirth2005.Network;
using Serilog;

namespace RuneRebirth2005.PlayerManagement;

public static class PlayerManager
{
    public static Player InitializeClient(TcpClient client)
    {
        var player = new Player(new PlayerSession(client));
        return player;
    }

    public static void DisconnectPlayer(Player player)
    {
        if (player.Index != -1)
            Server.Players[player.Index] = null;

        player.PlayerSession.Socket.Close();
        Log.Warning($"Client {player.Index} disconnected.");
    }

    public static void SilentDisconnectPlayer(Player player)
    {
        if (player.Index != -1)
            Server.Players[player.Index] = null;

        player.PlayerSession.Socket.Close();
    }

    public static void AssignAvailablePlayerSlot(Player player)
    {
        for (var i = 1; i < Server.Players.Length; i++)
            if (Server.Players[i] == null)
            {
                player.Index = i;
                Server.Players[i] = player;
                Log.Information($"Incoming connection has been assigned index: {player.Index}!");
                return;
            }

        Log.Warning($"Server is full! Disconnecting {player.PlayerSession.Socket.Client.RemoteEndPoint}.");
        DisconnectPlayer(player);
        throw new Exception("Server is full!");
    }

    public static void RegisterPlayer(Player player)
    {
        Server.Players[player.Index] = player;
        Log.Information($"Client with ID: [{player.Index}] has been registered!");
    }

    public static void Login(Player player)
    {
        player.PacketSender.LoadRegionPacket();
        player.PacketSender.SendPlayerStatus();

        player.PacketSender.SendSidebarInterface(0, 5855);
        player.PacketSender.SendTextToInterface("Unarmed", 5857);

        player.PacketSender.SendSidebarInterface(1, 3917);
        player.PacketSender.SendSidebarInterface(2, 638);
        player.PacketSender.SendSidebarInterface(3, 3213); /* Inventory */
        player.PacketSender.SendSidebarInterface(4, 1644);
        player.PacketSender.SendSidebarInterface(5, 5608);
        player.PacketSender.SendSidebarInterface(6, 1151);
        player.PacketSender.SendSidebarInterface(8, 5065);
        player.PacketSender.SendSidebarInterface(9, 5715);
        player.PacketSender.SendSidebarInterface(10, 2449);
        player.PacketSender.SendSidebarInterface(11, 4445);
        player.PacketSender.SendSidebarInterface(12, 147);
        player.PacketSender.SendSidebarInterface(13, 6299);

        player.IsUpdateRequired = true;
        player.Flags |= PlayerUpdateFlags.Appearance;
    }
}