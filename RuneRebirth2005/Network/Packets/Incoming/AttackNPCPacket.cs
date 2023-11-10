using RuneRebirth2005.Entities;
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
        _player.InteractingEntityId = _entityId;
        _player.CombatFocus = npc;
        
        _player.Flags |= PlayerUpdateFlags.InteractingEntity;
        _player.IsUpdateRequired = true;
        
        Log.Information($"Attack NPC - ID:{_entityId}");
        
    }
}