using RuneRebirth2005.Entities;
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
        _player.Combat.Attack(npc);
    }
}