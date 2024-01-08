using System.Reflection;
using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;
using RuneRebirth2005.Network;
using RuneRebirth2005.World;
using RuneRebirth2005.World.Clipping;

namespace RuneRebirth2005.Fighting;

public class PlayerCombat : CombatBase
{
    private readonly Player _character;

    public PlayerCombat(Player character) : base(character)
    {
        _character = character;
    }

    public override void DoAttack()
    {
        if (Target != null)
        {
            if (CanReach(_character, Target))
            {
                if (AttackTimer <= 0)
                {
                    /* Check if can combat */
                    if (CombatHelper.CanAttack(Character, Target))
                    {
                        /* Used to set the correct animations */
                        Target.Combat.WasHit = true;
                        Character.Combat.PerformedHit = true;

                        Target.Combat.HitQueue.AddHit(new CombatHit
                        {
                            Damage = 1,
                            HitType = 1,
                            Attacker = Character,
                            Target = Target,
                            Delay = 4
                        }, true);
                        AttackTimer = Character.AttackSpeed;
                        
                        if (Target != null)
                        {
                            var pX = Target.Location.X;
                            var pY = Target.Location.Y;

                            var nX = Character.Location.X;
                            var nY = Character.Location.Y;

                            var offX = (nY - pY) * -1;
                            var offY = (nX - pX) * -1;

                            _character.PacketSender.CreateProjectile(nX, nY, offX, offY, 50, 60, 10,
                                43, 31, -(Target.Index) - 1, 40);
                        }
                        
                    }
                }
            }
        }
    }

    private bool CanReach(Player attacker, Character target)
    {
        /* Check if we're using melee or mage / range */

        if (attacker.UsingBow)
        {
            /* Check If Range Path Blocked */
            if (ProjectilePathBlocked(attacker, target) || !attacker.Location.IsWithinDistance(target.Location, 8))
            {
                return false;
            }
        }
        else
        {
            /* Melee Path Blocked */
            if (MeleePathBlocked(attacker, target) || !attacker.Location.IsWithinDistance(target.Location, 1))
                return false;
        }

        attacker.MovementHandler.Reset();
        attacker.MovementHandler.FollowCharacter = null;
        return true;
    }

    private bool ProjectilePathBlocked(Character attacker, Character target)
    {
        var canProjectileMove = Region.canProjectileMove(attacker.Location.X, attacker.Location.Y, target.Location.X, target.Location.Y, attacker.Location.Z, 1, 1);
        if (!canProjectileMove)
        {
            attacker.MovementHandler.Reset();
            var tiles = PathFinder.getPathFinder().FindPath(attacker, target.Location.X, target.Location.Y, true, 8, 8);

            if (tiles != null)
            {
                for (var i = 0; i < tiles.Count; i++) attacker.MovementHandler.AddToPath(tiles[i]);
                /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
                attacker.MovementHandler.Finish();
            }

            return true;
        }

        return false;
    }

    private static bool MeleePathBlocked(Character attacker, Character target)
    {
        if (PathBlocked(attacker, target))
        {
            if (attacker.Location.X < target.Location.X && Region.GetClipping(target.Location.X - target.Size,
                    target.Location.Y, target.Location.Z, -1, 0))
            {
                attacker.MovementHandler.Reset();
                var tiles = PathFinder.getPathFinder()
                    .FindPath(attacker, target.Location.X, target.Location.Y, true, 1, 1);

                if (tiles != null)
                {
                    for (var i = 0; i < tiles.Count; i++) attacker.MovementHandler.AddToPath(tiles[i]);
                    /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
                    attacker.MovementHandler.Finish();
                }

                return true;
            }
            else if (attacker.Location.X > target.Location.X && Region.GetClipping(target.Location.X + target.Size,
                         target.Location.Y, target.Location.Z, -1, 0))
            {
                return true;
            }
            else if (attacker.Location.Y < target.Location.Y && Region.GetClipping(target.Location.X,
                         target.Location.Y - target.Size, target.Location.Z, -1, 0))
            {
                PathFinder.getPathFinder().FindPath(attacker, target.Location.X, target.Location.Y - 1, true, 1, 1);
                return true;
            }
            else if (attacker.Location.Y > target.Location.Y && Region.GetClipping(target.Location.X,
                         target.Location.Y + target.Size, target.Location.Z, -1, 0))
            {
                attacker.MovementHandler.Reset();
                var tiles = PathFinder.getPathFinder().FindPath(attacker, target.Location.X + target.Size,
                    target.Location.Y,
                    true, 1, 1);

                if (tiles != null)
                {
                    for (var i = 0; i < tiles.Count; i++) attacker.MovementHandler.AddToPath(tiles[i]);
                    /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
                    attacker.MovementHandler.Finish();
                }

                return true;
            }
            else if (Region.GetClipping(target.Location.X - 1, target.Location.Y, target.Location.Z, -1, 0))
            {
                PathFinder.getPathFinder().FindPath(attacker, target.Location.X - 1, target.Location.Y, true, 1, 1);
                return true;
            }
            else if (Region.GetClipping(target.Location.X + 1, target.Location.Y, target.Location.Z, -1, 0))
            {
                PathFinder.getPathFinder().FindPath(attacker, target.Location.X + 1, target.Location.Y, true, 1, 1);
                return true;
            }
            else if (Region.GetClipping(target.Location.X, target.Location.Y + 1, target.Location.Z, -1, 0))
            {
                PathFinder.getPathFinder().FindPath(attacker, target.Location.X, target.Location.Y + 1, true, 1, 1);
                return true;
            }
            else if (Region.GetClipping(target.Location.X, target.Location.Y - 1, target.Location.Z, -1, 0))
            {
                PathFinder.getPathFinder().FindPath(attacker, target.Location.X, target.Location.Y - 1, true, 1, 1);
                return true;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    public override void PerformAnimation()
    {
        if (Character.Combat.Target?.CurrentHealth <= 0)
        {
            var player = Character as Player;
            player.PacketSender.SendMessage("You've defeated your enemy!");
            player.MovementHandler.Reset();
            Reset();
        }

        if (!PerformedHit && WasHit)
        {
            Character.PerformAnimation(Character.BlockAnimation);
        }
        else if (PerformedHit)
        {
            Character.PerformAnimation(GetWepAnim(_character.Equipment.GetItem(EquipmentSlot.Weapon).Name));
        }


        

        if (Character.CurrentHealth <= 0)
        {
            Character.PerformAnimation(Character.FallAnimation);
            Reset();

            var player = Character as Player;
            DelayedTaskHandler.RegisterTask(new DelayedAttackTask
            {
                RemainingTicks = 4,
                Task = () => { player.Respawn(); }
            });
        }
    }

    public int GetWepAnim(string weaponName)
    {
        // if (playerEquipment[playerWeapon] <= 0)
        // {
        //     switch (fightMode)
        //     {
        //         case 0:
        //             return 422;
        //         case 2:
        //             return 423;
        //         case 1:
        //             return 451;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }
        // }

        // if (weaponName.Contains("knife")
        //     || weaponName.Contains("dart")
        //     || weaponName.Contains("javelin")
        //     || weaponName.Contains("thrownaxe"))
        // {
        //     return 806;
        // }
        //
        // if (weaponName.Contains("halberd"))
        // {
        //     return 440;
        // }
        //
        // if (weaponName.StartsWith("dragon dagger"))
        // {
        //     return 402;
        // }
        //
        // if (weaponName.EndsWith("dagger"))
        // {
        //     return 412;
        // }
        //
        // if (weaponName.Contains("2h sword")
        //     || weaponName.Contains("godsword")
        //     || weaponName.Contains("aradomin sword"))
        // {
        //     return 4307;
        // }
        //

        if (string.IsNullOrEmpty(weaponName))
        {
            return 422;
        }

        if (weaponName.Contains("sword"))
        {
            return 451;
        }

        //
        // if (weaponName.Contains("karil"))
        // {
        //     return 2075;
        // }
        //
        if (weaponName.Contains("bow") && !weaponName.Contains("'bow"))
        {
            return 426;
        }

        if (weaponName.Contains("'bow"))
        {
            return 4230;
        }

        switch (_character.Equipment.GetItem(EquipmentSlot.Weapon).ItemId)
        {
            case 6522:
                return 2614;
            case 4153:
                return 1665;
            case 4726:
                return 2080;
            case 4747:
                return 814;
            case 4718:
                return 2067;
            case 4710:
                return 406;
            case 4755:
                return 2062;
            case 4734:
                return 2075;
            case 4151:
                return 1658;
            case 6528:
                return 2661;
            default:
                return 451;
        }
    }

    public static bool PathBlocked(Character attacker, Character victim)
    {
        double offsetX = Math.Abs(attacker.Location.X - victim.Location.X);
        double offsetY = Math.Abs(attacker.Location.Y - victim.Location.Y);

        int distance = TileControl.CalculateDistance(attacker, victim);

        if (distance == 0)
        {
            return true;
        }

        offsetX = offsetX > 0 ? offsetX / distance : 0;
        offsetY = offsetY > 0 ? offsetY / distance : 0;

        var path = new int[distance][];

        int curX = attacker.Location.X;
        int curY = attacker.Location.Y;

        double currentTileXCount = 0.0;
        double currentTileYCount = 0.0;

        while (distance > 0)
        {
            distance--;
            int nextMoveX = 0;
            int nextMoveY = 0;

            if (curX > victim.Location.X)
            {
                currentTileXCount += offsetX;
                if (currentTileXCount >= 1.0)
                {
                    nextMoveX--;
                    curX--;
                    currentTileXCount -= offsetX;
                }
            }
            else if (curX < victim.Location.X)
            {
                currentTileXCount += offsetX;
                if (currentTileXCount >= 1.0)
                {
                    nextMoveX++;
                    curX++;
                    currentTileXCount -= offsetX;
                }
            }

            if (curY > victim.Location.Y)
            {
                currentTileYCount += offsetY;
                if (currentTileYCount >= 1.0)
                {
                    nextMoveY--;
                    curY--;
                    currentTileYCount -= offsetY;
                }
            }
            else if (curY < victim.Location.Y)
            {
                currentTileYCount += offsetY;
                if (currentTileYCount >= 1.0)
                {
                    nextMoveY++;
                    curY++;
                    currentTileYCount -= offsetY;
                }
            }

            path[distance] = new int[] { curX, curY, attacker.Location.Z, nextMoveX, nextMoveY };
        }

        for (int i = 0; i < path.Length; i++)
        {
            if (!Region.GetClipping(path[i][0], path[i][1], path[i][2], path[i][3], path[i][4]))
            {
                return true;
            }
        }

        return false;
    }

    public static bool PathBlockedP(Character attacker, Character victim)
    {
        double offsetX = Math.Abs(attacker.Location.X - victim.Location.X);
        double offsetY = Math.Abs(attacker.Location.Y - victim.Location.Y);

        int distance = TileControl.CalculateDistance(attacker, victim);

        if (distance == 0)
        {
            return true;
        }

        offsetX = offsetX > 0 ? offsetX / distance : 0;
        offsetY = offsetY > 0 ? offsetY / distance : 0;

        int[][] path = new int[distance][];
        for (int j = 0; j < distance; j++)
            path[j] = new int[7];

        int curX = attacker.Location.X;
        int curY = attacker.Location.Y;
        int next = 0;
        int nextMoveX = 0;
        int nextMoveY = 0;

        double currentTileXCount = 0.0;
        double currentTileYCount = 0.0;

        while (distance > 0)
        {
            distance--;
            nextMoveX = 0;
            nextMoveY = 0;
            if (curX > victim.Location.X)
            {
                currentTileXCount += offsetX;
                if (currentTileXCount >= 1.0)
                {
                    nextMoveX--;
                    curX--;
                    currentTileXCount -= offsetX;
                }
            }
            else if (curX < victim.Location.X)
            {
                currentTileXCount += offsetX;
                if (currentTileXCount >= 1.0)
                {
                    nextMoveX++;
                    curX++;
                    currentTileXCount -= offsetX;
                }
            }

            if (curY > victim.Location.Y)
            {
                currentTileYCount += offsetY;
                if (currentTileYCount >= 1.0)
                {
                    nextMoveY--;
                    curY--;
                    currentTileYCount -= offsetY;
                }
            }
            else if (curY < victim.Location.Y)
            {
                currentTileYCount += offsetY;
                if (currentTileYCount >= 1.0)
                {
                    nextMoveY++;
                    curY++;
                    currentTileYCount -= offsetY;
                }
            }

            path[next][0] = curX;
            path[next][1] = curY;
            path[next][2] = attacker.Location.Z;
            path[next][3] = nextMoveX;
            path[next][4] = nextMoveY;
            path[next][5] = attacker.Location.X;
            path[next][6] = attacker.Location.Y;
            next++;
        }

        for (int i = 0; i < path.Length; i++)
        {
            if (Region.BlockedShot(path[i][0], path[i][1], path[i][2], path[i][3], path[i][4]))
            {
                return true;
            }
        }

        return false;
    }
}