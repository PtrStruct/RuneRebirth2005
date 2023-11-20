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
        // _entityId = _player.Reader.ReadSignedWordA();
    }

    public void Process()
    {
        var npc = Server.NPCs[_entityId];
        /* Set Target */
        /* Set InteractingEntity*/
        // _player.Attack(npc);

        // if (npc.CurrentHealth <= 0)
        //     return;
        //
        // if (_player.NPCCombatFocus != npc && _player.NPCCombatFocus != null)
        // {
        //     new SendPlayerMessagePacket(_player).Add("You're already in combat.");
        //     return;
        // }
        //
        // if (npc.PlayerCombatFocus == null)
        // {
        //     _player.InteractingEntityId = _entityId;
        //     _player.NPCCombatFocus = npc;
        //
        //     _player.Flags |= PlayerUpdateFlags.InteractingEntity;
        //     _player.IsUpdateRequired = true;
        //
        //     Log.Information($"Attack NPC - ID:{_entityId}");
        // }
        // else if (npc.PlayerCombatFocus != _player)
        // {
        //     new SendPlayerMessagePacket(_player).Add("That target is already in combat.");
        // }
    }
}