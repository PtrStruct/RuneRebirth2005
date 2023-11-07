using System.Net.Sockets;
using RuneRebirth2005.ClientManagement;
using RuneRebirth2005.Entities;
using RuneRebirth2005.Network;
using RuneRebirth2005.Network.Outgoing;
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
        Log.Information($"Initialized new Player");
        return player;
    }

    public static void DisconnectClient(Client client)
    {
        if (client.Index != -1)
            Server.Players[client.Index] = new Player();

        client.Socket.Close();
        Log.Warning($"Client {client.Index} disconnected.");
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
        Log.Information($"Client with ID: [{player.Index}] has been registered!");
    }

    public static void Login(Player player)
    {
        player.IsUpdateRequired = true;
        player.DidTeleportOrSpawn = true;
        player.Flags |= PlayerUpdateFlags.Appearance;
        player.LoadPlayer();

        new RegionLoadPacket(player).Add();
        new SendPlayerStatusPackets(player).Add();

        new SetSidebarInterfacePacket(player).Add(0, 5855);
        new TextToInterfacePacket(player).Add("Unarmed", 5857);

        new SetSidebarInterfacePacket(player).Add(1, 3917);
        new SetSidebarInterfacePacket(player).Add(2, 638);
        new SetSidebarInterfacePacket(player).Add(3, 3213); /* Inventory */
        new SetSidebarInterfacePacket(player).Add(4, 1644);
        new SetSidebarInterfacePacket(player).Add(5, 5608);
        new SetSidebarInterfacePacket(player).Add(6, 1151);
        new SetSidebarInterfacePacket(player).Add(8, 5065);
        new SetSidebarInterfacePacket(player).Add(9, 5715);
        new SetSidebarInterfacePacket(player).Add(10, 2449);
        new SetSidebarInterfacePacket(player).Add(11, 4445);
        new SetSidebarInterfacePacket(player).Add(12, 147);
        new SetSidebarInterfacePacket(player).Add(13, 6299);

        foreach (SkillEnum skill in Enum.GetValues(typeof(SkillEnum)))
        {
            int experience = player.Data.PlayerSkills.Experience[(int)skill];
            int level = player.Data.PlayerSkills.Levels[(int)skill];

            new SetSkillLevelPacket(player).Add(skill, experience, level);
        }
    }
}