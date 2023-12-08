using RuneRebirth2005.Entities;

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
            if (CanReach(Character, Target))
            {
                if (AttackTimer <= 0)
                {
                    /* Check if can combat */
                    if (CombatHelper.CanAttack(Character, Target))
                    {
                        Character.Combat.PerformedHit = true;
                        Target.Combat.WasHit = true;

                        Target.Combat.HitQueue.AddHit(new CombatHit
                        {
                            Damage = 1,
                            HitType = 1,
                            Attacker = Character,
                            Target = Target
                        }, true);
                        AttackTimer = Character.AttackSpeed;
                    }
                }
            }
        }
    }

    private bool CanReach(Character attacker, Character target)
    {
        return attacker.Location.IsWithinDistance(target.Location, 1);
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
}