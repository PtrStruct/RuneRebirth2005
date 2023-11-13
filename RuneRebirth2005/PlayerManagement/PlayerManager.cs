using System.Net.Sockets;
using RuneRebirth2005.ClientManagement;
using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;
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
        return player;
    }

    public static void DisconnectClient(Client client)
    {
        if (client.Index != -1)
            Server.Players[client.Index] = new Player();

        client.Socket.Close();
        Log.Warning($"Client {client.Index} disconnected.");
    }

    public static void SilentDisconnectClient(Client client)
    {
        if (client.Index != -1)
            Server.Players[client.Index] = new Player();

        client.Socket.Close();
    }

    public static void AssignAvailablePlayerSlot(Player player)
    {
        for (var i = 0; i < Server.Players.Length; i++)
            if (Server.Players[i].Index == -1)
            {
                player.Index = i + 1;
                Log.Information($"Incoming connection has been assigned index: {player.Index}!");
                return;
            }

        Log.Warning($"Server is full! Disconnecting {player.Socket.Client.RemoteEndPoint}.");
        DisconnectClient(player);
        throw new Exception("Server is full!");
    }

    public static void RegisterPlayer(Player player)
    {
        Server.Players[player.Index - 1] = player;
        Log.Information($"Client with ID: [{player.Index}] has been registered!");
    }

    public static void Login(Player player)
    {
        player.IsUpdateRequired = true;
        player.DidTeleportOrSpawn = true;
        player.Flags |= PlayerUpdateFlags.Appearance;
        player.LoadPlayer();

        player.CalculateCombatLevel();
        
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

        new SetPlayerOptionsPacket(player).Add(4, false, "Trade with");
        new SetPlayerOptionsPacket(player).Add(5, false, "Follow");

        for (int i = 0; i < BonusHelper.BonusMap.Count; i++)
        {
            var bonus = BonusHelper.BonusMap[i];

            int bonusValue = player.Data.Bonuses.GetBonus(bonus.Index);
            string sign = bonusValue > 0 ? "+" : "";
            new TextToInterfacePacket(player).Add($"{bonus.Name}: {sign}{bonusValue}", BonusHelper.BonusMap[i].FrameId);
        }

        foreach (EquipmentSlot equipment in Enum.GetValues(typeof(EquipmentSlot)))
        {
            var item = player.Data.Equipment.GetItem(equipment);
            new UpdateSlotPacket(player).Add(equipment, item.ItemId, item.Quantity);
        }

        BonusManager.RefreshBonus(player);

        foreach (SkillEnum skill in Enum.GetValues(typeof(SkillEnum)))
        {
            var theSkill = player.Data.PlayerSkills.GetSkill(skill);
            if (skill == SkillEnum.Hitpoints)
            {
                new SetSkillLevelPacket(player).Add(skill, theSkill.Experience, player.Data.CurrentHealth);
                continue;
            }
            
            new SetSkillLevelPacket(player).Add(skill, theSkill.Experience, theSkill.Level);
        }
    }
}