﻿using RuneRebirth2005.Entities;
using RuneRebirth2005.Network.Outgoing;
using RuneRebirth2005.NPCManagement;
using Serilog;

namespace RuneRebirth2005.Network.Incoming;

public class AttackNPCPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _entityId;

    public AttackNPCPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        _entityId = _player.Reader.ReadSignedWordA();
    }
    
    public void Process()
    {
        var npc = NPCManager.WorldNPCs[_entityId];

        if (npc.InCombat)
        {
            new SendPlayerMessagePacket(_player).Add("That target is already in combat.");
            return;
        }
        
        if (_player.InCombat)
        {
            new SendPlayerMessagePacket(_player).Add("You're already in combat.");
            return;
        }
        
        _player.InteractingEntityId = _entityId;
        _player.NPCCombatFocus = npc;
        
        _player.Flags |= PlayerUpdateFlags.InteractingEntity;
        _player.IsUpdateRequired = true;
        
        Log.Information($"Attack NPC - ID:{_entityId}");
        
    }
}