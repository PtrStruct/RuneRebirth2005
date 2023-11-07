using RuneRebirth2005.ClientManagement;
using Serilog;

namespace RuneRebirth2005.Network;

public class LoginHandler
{
    private readonly Client _client;

    public LoginHandler(Client client)
    {
        _client = client;
    }
    
    public bool Handshake()
    {
        var serverSessionKey = SessionEncryption.GenerateServerSessionKey();

        _client.FillStream(2);
        var connectionType = _client.Reader.ReadUnsignedByte();
        Log.Information($"ConnectionType: {connectionType}");
        var userHash = _client.Reader.ReadUnsignedByte();

        for (var i = 0; i < 8; i++)
            _client.Writer.WriteByte(0);

        _client.Writer.WriteByte(0); //responseCode 0 - Exchanges session keys, player name, password, etc. 
        _client.Writer.WriteQWord(serverSessionKey);
        _client.FlushBufferedData(); /* Send */

        _client.FillStream(2);
        var connectionStatus = _client.Reader.ReadUnsignedByte();
        var loginPacketSize = _client.Reader.ReadUnsignedByte();
        _client.FillStream(loginPacketSize);

        var loginEncryptPacketSize = loginPacketSize - (36 + 1 + 1 + 2);
        var magicNumber = _client.Reader.ReadUnsignedByte();
        var revision = _client.Reader.ReadSignedWord();
        var clientVersion = _client.Reader.ReadUnsignedByte();

        var crcValues = new int[9];
        for (var i = 0; i < crcValues.Length; i++)
            crcValues[i] = _client.Reader.ReadDWord();

        var size2 = _client.Reader.ReadUnsignedByte();
        var magicNumber2 = _client.Reader.ReadUnsignedByte();

        var ISAACSeed = new int[4];
        for (var i = 0; i < ISAACSeed.Length; i++)
            ISAACSeed[i] = _client.Reader.ReadDWord();
        _client.InEncryption = new SessionEncryption(ISAACSeed);

        for (var i = 0; i < ISAACSeed.Length; i++)
            ISAACSeed[i] += 50;
        _client.OutEncryption = new SessionEncryption(ISAACSeed);
        _client.Writer.packetEncryption = _client.OutEncryption;

        var UID = _client.Reader.ReadDWord();
        _client.Data.Username = _client.Reader.ReadString();
        _client.Data.Password = _client.Reader.ReadString();

        if (Server.Players.Any(player => string.Equals(player?.Data.Username, _client.Data.Username, StringComparison.CurrentCultureIgnoreCase)))
        {
            Log.Information($"{_client.Data.Username} tried logging in even though they're already logged in.");
            _client.Writer.WriteByte(5);
            _client.FlushBufferedData();
            return false;
        }

        _client.Writer.WriteByte(2); /* Secondary response code 2 = Login | 5 = Already logged in etc. */
        _client.Writer.WriteByte(2);
        _client.Writer.WriteByte(0);
        _client.FlushBufferedData();

        return true;
    }
}