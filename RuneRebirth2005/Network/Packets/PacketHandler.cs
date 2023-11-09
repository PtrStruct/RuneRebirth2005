using RuneRebirth2005.Entities;
using Serilog;

namespace RuneRebirth2005.Network;

public class PacketHandler(Player player)
{

    private enum State
    {
        READ_OPCODE = 0,
        READ_VAR_SIZE = 1,
        READ_PAYLOAD = 2
    }

    private State _state = State.READ_OPCODE;
    
    private int _opCode = -1;
    private int _packetLength = -1;
    public void RetrievePacket()
    {
        if (!_player.NetworkStream.DataAvailable)
            return;

        FillStream(1);

        if (_opCode == -1)
            UpdatePacketOpCode();

        if (_opCode != 0)
        {
            Log.Information($"Received opcode {_opCode}.");
        }

        if (_state == State.READ_OPCODE)
        {
            if (player.Socket.Available == 0)
            {
                return;
            }

            player.FillStream(1);
            
            _opCode = (byte)(GetReader().ReadUnsignedByte() - GetInEncryptionKey());
            _packetLength = GameConstants.INCOMING_SIZES[_opCode];

            _state = _packetLength switch
            {
                0 =>
                    // we should add this packet to the player even if its empty
                    State.READ_OPCODE,
                -1 => State.READ_VAR_SIZE,
                _ => State.READ_PAYLOAD
            };
        }

        if (_state == State.READ_VAR_SIZE)
        {
            if (player.Socket.Available == 0)
            {
                return;
            }

            player.FillStream(1);

            _packetLength = GetReader().ReadUnsignedByte();
            _state = State.READ_PAYLOAD;
        }

        if (_state != State.READ_PAYLOAD) return;
        
        if (_packetLength > player.Socket.Available)
        {
            return;
        }
        player.FillStream(_packetLength);
        Log.Information($"[{_opCode}] Packet Received - Length: {_packetLength}");

        var packet = PacketFactory.CreateClientPacket(_opCode,
            new PacketParameters { OpCode = _opCode, Length = _packetLength, Player = player });
            
        player.PacketStore.AddPacket(_opCode, packet);
        _state = State.READ_OPCODE;
    }

    private RSStream GetReader()
    {
        return player.Reader;
    }

    private int GetInEncryptionKey()
    {
        return player.InEncryption.GetNextKey();
    }
}

public class PacketParameters
{
    public int OpCode { get; set; }
    public int Length { get; set; }
    public Player Player { get; set; }
}