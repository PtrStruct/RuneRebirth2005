using RuneRebirth2005.Entities;
using RuneRebirth2005.Fighting;
using RuneRebirth2005.NPCManagement;
using Serilog;

namespace RuneRebirth2005.Network.Incoming;

public class AttackNPCPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _entityIndex;

    public AttackNPCPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        _entityIndex = _player.PlayerSession.Reader.ReadSignedWordA();
    }

    public void Process()
    {
        var npc = Server.NPCs.FirstOrDefault(x => x.Index == _entityIndex);

        /* Only do this check if we're not in multi */
        if (CombatHelper.IsAttacked(_player) && _player.Combat.Attacker != npc)
        {
            _player.PacketSender.SendMessage("You are already under attack!");
            return;
        }

        if (CombatHelper.IsAttacked(npc) && npc.Combat.Attacker != _player)
        {
            _player.PacketSender.SendMessage("They are already under attack!");
            return;
        }

        _player.Combat.Attack(npc);
    }
}