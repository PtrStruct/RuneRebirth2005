using RuneRebirth2005.Entities;
using Serilog;

namespace RuneRebirth2005.Network;

public class PacketHandler
{
    private int _opCode = -1;
    private int _packetLength = -1;
    private readonly Player _player;

    public PacketHandler(Player player)
    {
        _player = player;
    }

    public void RetrievePacket()
    {
        if (!_player.NetworkStream.DataAvailable)
            return;

        FillStream(1);

        if (_opCode == -1)
            UpdatePacketOpCode();

        Log.Information($"Received opcode {_opCode}.");

        if (_packetLength == -1)
        {
            SetPacketLength();

            if (_packetLength == 0)
            {
                ResetOpCodeAndLength();
                return;
            }

            if (_packetLength == -1)
            {
                FillStream(1);
                _packetLength = GetReader().ReadSignedByte();
            }

            FillStream(_packetLength);
            Log.Information($"[{_opCode}] Packet Received - Length: {_packetLength}");
        }

        var packet = PacketFactory.CreateClientPacket(_opCode,
            new PacketParameters { OpCode = _opCode, Length = _packetLength, Player = _player });

        _player.PacketStore.AddPacket(_opCode, packet);
        ResetOpCodeAndLength();
    }

    private void SetPacketLength()
    {
        _packetLength = GameConstants.INCOMING_SIZES[_opCode];
    }

    private void UpdatePacketOpCode()
    {
        _opCode = (byte)(GetReader().ReadSignedByte() - GetInEncryptionKey());
    }

    private void ResetOpCodeAndLength()
    {
        _opCode = -1;
        _packetLength = -1;
    }

    private RSStream GetReader()
    {
        return _player.Reader;
    }

    private int GetInEncryptionKey()
    {
        return _player.InEncryption.GetNextKey();
    }

    private void FillStream(int i)
    {
        _player.FillStream(i);
    }

}

public class PacketParameters
{
    public int OpCode { get; set; }
    public int Length { get; set; }
    public Player Player { get; set; }
}