using System.Net.Sockets;
using RuneRebirth2005.Entities;
using Serilog;

namespace RuneRebirth2005.Network;

public class PlayerSession
{
    public RSStream Reader { get; set; }
    public RSStream Writer { get; set; }
    public TcpClient Socket { get; set; }
    public NetworkStream NetworkStream { get; set; }
    public SessionEncryption InEncryption { get; set; }
    public SessionEncryption OutEncryption { get; set; }
    public PacketStore PacketStore { get; set; }

    public PlayerSession(TcpClient tcpClient)
    {
        Socket = tcpClient;
        NetworkStream = tcpClient.GetStream();
        Reader = new RSStream(new byte[ServerConfig.BUFFER_SIZE]);
        Writer = new RSStream(new byte[ServerConfig.BUFFER_SIZE]);
        PacketStore = new PacketStore();
    }

    private enum State
    {
        READ_OPCODE = 0,
        READ_VAR_SIZE = 1,
        READ_PAYLOAD = 2
    }

    private State _state = State.READ_OPCODE;
    
    private int _opCode = -1;
    private int _packetLength = -1;
    
    public void Fetch(Player player)
    {
        if (_state == State.READ_OPCODE)
        {
            if (Socket.Available == 0)
            {
                return;
            }

            FillStream(1);
            
            _opCode = (byte)(Reader.ReadUnsignedByte() - InEncryption.GetNextKey());
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
            if (Socket.Available == 0)
            {
                return;
            }

            FillStream(1);

            _packetLength = Reader.ReadUnsignedByte();
            _state = State.READ_PAYLOAD;
        }

        if (_state != State.READ_PAYLOAD) return;
        
        if (_packetLength > Socket.Available)
        {
            return;
        }
        FillStream(_packetLength);
        Log.Information($"[{_opCode}] Packet Received - Length: {_packetLength}");

        var packet = PacketFactory.CreateClientPacket(_opCode,
            new PacketParameters { OpCode = _opCode, Length = _packetLength, Player = player });
            
        PacketStore.AddPacket(_opCode, packet);
        _state = State.READ_OPCODE;
    }
    
    public bool FillStream(int count)
    {
        if (!NetworkStream.CanRead)
            return false;

        try
        {
            Reader.CurrentOffset = 0;
            var bytes = NetworkStream.Read(Reader.Buffer, 0, count);

            if (bytes < count)
            {
                throw new IOException("we couldn't read enough bytes");
            }

            return true;
        }
        catch (IOException ex)
        {
            Disconnect(ex.Message);
            return false;
        }
        catch (ObjectDisposedException e)
        {
            Disconnect(e.Message);
            return false;
        }
    }

    public bool FlushBufferedData()
    {
        if (!NetworkStream.CanWrite)
            return false;

        try
        {
            NetworkStream.Write(Writer.Buffer, 0, Writer.CurrentOffset);
            NetworkStream.Flush();
            Writer.CurrentOffset = 0;
            return true;
        }
        catch (IOException ex)
        {
            Disconnect(ex.Message);
            return false;
        }
        catch (ObjectDisposedException e)
        {
            Disconnect(e.Message);
            return false;
        }
    }

    public void Disconnect(string msg)
    {
        Socket.Close();
        Log.Fatal(msg);
    }
}