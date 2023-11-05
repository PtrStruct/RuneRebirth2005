using RuneRebirth2005.Entities;
using RuneRebirth2005.Network.Outgoing;
using Serilog;

namespace RuneRebirth2005.Network.Incoming;

public class PlayerCommandPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly string[] _commandArgs;

    public PlayerCommandPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        _commandArgs = _player.Reader.ReadString().Split(' ');
    }

    public void Process()
    {
        var command = _commandArgs[0];
        Log.Information($"Player sent command: {command}");

        switch (command)
        {
            case "pos":
                foreach (var message in _player.Location.ToStringParts())
                    new SendPlayerMessagePacket(_player).Add(message);
                break;
            case "hair":
                var style = int.Parse(_commandArgs[1]);
                var color = int.Parse(_commandArgs[2]);
                _player.Appearance.Hair = style;
                _player.Colors.Hair = color;
                _player.IsUpdateRequired = true;
                _player.Flags |= PlayerUpdateFlags.Appearance;
                break;
        }
    }
}