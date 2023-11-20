using RuneRebirth2005.Entities;
using RuneRebirth2005.NPCManagement;
using RuneRebirth2005.PlayerManagement;
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
        _commandArgs = _player.PlayerSession.Reader.ReadString().Split(' ');
    }

    public void Process()
    {
        var command = _commandArgs[0];
        Log.Information($"Player sent command: {command}");

        switch (command)
        {
            case "pos":
                foreach (var message in _player.Location.ToStringParts())
                {
                    // new SendPlayerMessagePacket(_player).Add(message);
                }
                break;
            case "hair":
                var style = int.Parse(_commandArgs[1]);
                var color = int.Parse(_commandArgs[2]);
                _player.Appearance.Hair = style;
                _player.Colors.Hair = color;
                _player.IsUpdateRequired = true;
                _player.Flags |= PlayerUpdateFlags.Appearance;
                break;
            case "level":
                var skillId = int.Parse(_commandArgs[1]);
                var level = int.Parse(_commandArgs[2]);

                // Check if the provided skill id is valid
                if (skillId >= 0 && skillId < Enum.GetNames(typeof(SkillEnum)).Length)
                {
                    _player.PlayerSkills.SetSkill((SkillEnum)skillId, level);
                    _player.SavePlayer();
                }
                else
                {
                    // new SendPlayerMessagePacket(_player).Add("Invalid skill ID provided");
                }

                break;

            case "equip":
                var itemId = int.Parse(_commandArgs[1]);
                _player.Equipment.EquipItem(itemId);
                _player.IsUpdateRequired = true;
                BonusManager.RefreshBonus(_player);
                _player.SavePlayer();
                break;


            case "kill":
                // var npcIndex = int.Parse(_commandArgs[1]);
                // var npc = NPCManager.WorldNPCs[npcIndex];
                // npc.CurrentAnimation = npc.FallAnimation;
                // npc.Flags |= NPCUpdateFlags.Animation;
                // npc.IsUpdateRequired = true;
                break;

            case "unkill":
                // npcIndex = int.Parse(_commandArgs[1]);
                // npc = NPCManager.WorldNPCs[npcIndex];
                // npc.Alive = true;
                // npc.CurrentAnimation = -1;
                // npc.Flags |= NPCUpdateFlags.Animation;
                // npc.IsUpdateRequired = true;
                break;

            case "logout":
                _player.SavePlayer();
                // new LogoutPacket(_player).Add();
                break;
        }
    }
}