using RuneRebirth2005.Entities;
using RuneRebirth2005.PlayerManagement;
using Serilog;

namespace RuneRebirth2005.Network;

public class LoginHandler
{
    private Player _player;
    private readonly PlayerSession _session;

    public LoginHandler(Player player)
    {
        _player = player;
        _session = _player.PlayerSession;
    }

    public bool Handshake()
    {
        var serverSessionKey = SessionEncryption.GenerateServerSessionKey();

        if (!_session.FillStream(2))
        {
            PlayerManager.DisconnectPlayer(_player);
            return false;
        }
        var connectionType = _session.Reader.ReadUnsignedByte();
        Log.Information($"ConnectionType: {connectionType}");
        var userHash = _session.Reader.ReadUnsignedByte();

        for (var i = 0; i < 8; i++)
            _session.Writer.WriteByte(0);

        _session.Writer.WriteByte(0); //responseCode 0 - Exchanges session keys, player name, password, etc. 
        _session.Writer.WriteQWord(serverSessionKey);

        if (!_session.FlushBufferedData())
        {
            PlayerManager.DisconnectPlayer(_player);
            return false;
        }

        if (!_session.FillStream(2))
        {
            PlayerManager.DisconnectPlayer(_player);
            return false;
        }
        var connectionStatus = _session.Reader.ReadUnsignedByte();
        var loginPacketSize = _session.Reader.ReadUnsignedByte();
        if (!_session.FillStream(loginPacketSize))
        {
            PlayerManager.DisconnectPlayer(_player);
            return false;
        }

        var loginEncryptPacketSize = loginPacketSize - (36 + 1 + 1 + 2);
        var magicNumber = _session.Reader.ReadUnsignedByte();
        var revision = _session.Reader.ReadSignedWord();
        var clientVersion = _session.Reader.ReadUnsignedByte();

        var crcValues = new int[9];
        for (var i = 0; i < crcValues.Length; i++)
            crcValues[i] = _session.Reader.ReadDWord();

        var size2 = _session.Reader.ReadUnsignedByte();
        var magicNumber2 = _session.Reader.ReadUnsignedByte();

        var ISAACSeed = new int[4];
        for (var i = 0; i < ISAACSeed.Length; i++)
            ISAACSeed[i] = _session.Reader.ReadDWord();
        _session.InEncryption = new SessionEncryption(ISAACSeed);

        for (var i = 0; i < ISAACSeed.Length; i++)
            ISAACSeed[i] += 50;
        _session.OutEncryption = new SessionEncryption(ISAACSeed);
        _session.Writer.packetEncryption = _session.OutEncryption;

        var UID = _session.Reader.ReadDWord();
        _player.Username = _session.Reader.ReadString();
        _player.Password = _session.Reader.ReadString();

        if (Server.Players.Any(player =>
                string.Equals(player?.Username, _player.Username, StringComparison.CurrentCultureIgnoreCase)))
        {
            Log.Information($"{_player.Username} tried logging in even though they're already logged in.");
            _session.Writer.WriteByte(5);

            if (!_session.FlushBufferedData())
            {
                PlayerManager.DisconnectPlayer(_player);
                return false;
            }

            return false;
        }

        _session.Writer.WriteByte(2); /* Secondary response code 2 = Login | 5 = Already logged in etc. */
        _session.Writer.WriteByte(2);
        _session.Writer.WriteByte(0);
        if (!_session.FlushBufferedData())
        {
            PlayerManager.DisconnectPlayer(_player);
            return false;
        }

        return true;
    }
}