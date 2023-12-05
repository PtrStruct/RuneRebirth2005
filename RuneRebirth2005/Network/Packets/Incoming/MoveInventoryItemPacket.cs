using System.Runtime.CompilerServices;
using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Incoming;


public class MoveInventoryItemPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _interfaceId;
    private readonly byte _insertionMode;
    private readonly int _from;
    private readonly int _to;

    public MoveInventoryItemPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;

        _interfaceId = _player.PlayerSession.Reader.ReadSignedWordA();
        _insertionMode = _player.PlayerSession.Reader.ReadSignedByteC();
        _from = _player.PlayerSession.Reader.ReadSignedWordBigEndianA();
        _to = _player.PlayerSession.Reader.ReadSignedWordBigEndian();
    }

    public void Process()
    {
        var from = _player.PlayerInventory.Inventory[_from];
        var to = _player.PlayerInventory.Inventory[_to];
        if (from.ItemId == -1) return;
        var temp = from;
        
        _player.PlayerInventory.Inventory[_from] = to;
        _player.PlayerInventory.Inventory[_to] = temp;
        
        _player.IsAppearanceUpdate = true;
    }
}