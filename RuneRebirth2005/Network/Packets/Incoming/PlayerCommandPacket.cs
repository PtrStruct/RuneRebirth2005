using RuneRebirth2005.Data.Items;
using RuneRebirth2005.Entities;
using RuneRebirth2005.Fighting;
using RuneRebirth2005.Helpers;
using RuneRebirth2005.NPCManagement;
using RuneRebirth2005.PlayerManagement;
using RuneRebirth2005.World;
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
                    _player.PacketSender.SendMessage(message);
                }
                
                _player.PacketSender.SendMessage($"Clipping: {Region.GetClipping(_player.Location.X, _player.Location.Y, _player.Location.Z)}");
                _player.PacketSender.SendMessage($"GetProjectileClipping /w South Mask: {(Region.GetProjectileClipping(_player.Location.X, _player.Location.Y, _player.Location.Z) & 0x1280102)}");
                _player.PacketSender.SendMessage($"GetProjectileClipping: {Region.GetProjectileClipping(_player.Location.X, _player.Location.Y, _player.Location.Z)}");
                _player.PacketSender.SendMessage($"ProjectileBlockedSouth: {Region.ProjectileBlockedSouth(_player.Location.X, _player.Location.Y, _player.Location.Z)}");

                break;

            case "respawn":
                _player.Respawn();
                break;

            case "npcwalk":
                var npcIndex = int.Parse(_commandArgs[1]);
                var npc = Server.NPCs[npcIndex];
                if (npc != null)
                {
                    npc.MovementHandler.AddToPath(new Location(npc.Location.X, npc.Location.Y + 1));
                }
                else
                {
                    _player.PacketSender.SendMessage($"No NPC with index {npcIndex}");
                }

                break;

            case "npcfollow":
                npcIndex = int.Parse(_commandArgs[1]);
                npc = Server.NPCs[npcIndex];
                if (npc != null)
                {
                    npc.MovementHandler.FollowCharacter = _player;
                }
                else
                {
                    _player.PacketSender.SendMessage($"No NPC with index {npcIndex}");
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

            case "cast":
                var gfx = int.Parse(_commandArgs[1]);
                npcIndex = int.Parse(_commandArgs[2]);
                npc = Server.NPCs[npcIndex];

                // _player.Flags |= PlayerUpdateFlags.Graphics;
                // npc.GraphicsId = 220;

                /* Origin */

                int pX = _player.Location.X;
                int pY = _player.Location.Y;

                int nX = npc.Location.X;
                int nY = npc.Location.Y;

                int offX = (pY - nY);
                int offY = (pX - nX);

                _player.SetInteractionEntity(npc);
                _player.PerformAnimation(711);
                _player.PacketSender.CreateProjectile(pX, pY, offX, offY, 50, 78, 91, 43, 31, (npc.Index + 1), 50);

                DelayedTaskHandler.RegisterTask(new DelayedAttackTask
                {
                    RemainingTicks = 3,
                    Task = () =>
                    {
                        npc.Combat.HitQueue.AddHit(new CombatHit
                        {
                            Attacker = _player,
                            Damage = 1,
                            Target = npc,
                            HitType = 1
                        }, true);
                    }
                });

                break;

            case "npccast":
                gfx = int.Parse(_commandArgs[1]);
                npcIndex = int.Parse(_commandArgs[2]);
                npc = Server.NPCs[npcIndex];

                var b = NpcHelper.npcs.FirstOrDefault(x => x.Value.Id == npc.ModelId).Value.ProjectileId;

                // _player.Flags |= PlayerUpdateFlags.Graphics;
                // npc.GraphicsId = 220;

                /* Origin */
                //91
                pX = _player.Location.X;
                pY = _player.Location.Y;

                nX = npc.Location.X;
                nY = npc.Location.Y;

                offX = (pY - nY);
                offY = (pX - nX);

                npc.SetInteractionEntity(npc);
                npc.PerformAnimation(711);
                _player.PacketSender.CreateProjectile(nX, nY, offX, offY, 50, 78, b, 43, 31, (_player.Index - 1), 50);

                DelayedTaskHandler.RegisterTask(new DelayedAttackTask
                {
                    RemainingTicks = 3,
                    Task = () =>
                    {
                        _player.Combat.HitQueue.AddHit(new CombatHit
                        {
                            Attacker = npc,
                            Damage = 1,
                            Target = _player,
                            HitType = 1
                        }, true);
                    }
                });

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
                var def = ItemDefinition.Lookup(itemId);
                _player.Equipment.EquipItem(itemId,def.Name);
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