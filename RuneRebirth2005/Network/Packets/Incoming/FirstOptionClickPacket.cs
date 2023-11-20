using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Incoming;

public class FirstOptionClickPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _interfaceId;
    private readonly int _index;
    private readonly int _itemId;

    public FirstOptionClickPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        
        // _interfaceId = _player.Reader.ReadSignedWordBigEndianA();
        // _index = _player.Reader.ReadSignedWordA();
        // _itemId = _player.Reader.ReadSignedWordBigEndian();
        
        // var instantWriter = new RSStream(new byte[2048]);
        // instantWriter.CreateFrame(ServerOpCodes.OBJ_ADD);
        // _player.NetworkStream.Write(instantWriter.Buffer, 0, instantWriter.CurrentOffset);
        // instantWriter.CurrentOffset = 0;
        // _player.NetworkStream.Flush();
    }
    
    public void Process()
    {
        // new SetInventorySlotPacket(_player).Add();
    }
}