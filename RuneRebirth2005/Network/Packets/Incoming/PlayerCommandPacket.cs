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
                foreach (var message in _player.Data.Location.ToStringParts())
                    new SendPlayerMessagePacket(_player).Add(message);
                break;
            case "hair":
                var style = int.Parse(_commandArgs[1]);
                var color = int.Parse(_commandArgs[2]);
                _player.Data.Appearance.Hair = style;
                _player.Data.Colors.Hair = color;
                _player.IsUpdateRequired = true;
                _player.Flags |= PlayerUpdateFlags.Appearance;
                break;
            case "level":
                var skillId = int.Parse(_commandArgs[1]);
                var level = int.Parse(_commandArgs[2]);

                // Check if the provided skill id is valid
                if (skillId >= 0 && skillId < Enum.GetNames(typeof(PlayerSkills.Skill)).Length)
                {
                    _player.Data.PlayerSkills.Levels[skillId] = level;
                    _player.SavePlayer();
                }
                else
                    new SendPlayerMessagePacket(_player).Add("Invalid skill ID provided");

                break;

            case "logout":
                _player.SavePlayer();
                new LogoutPacket(_player).Add();
                break;
        }
    }
}