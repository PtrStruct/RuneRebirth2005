using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Fighting;

public class CombatHelper
{
    public static bool CanAttack(Character attacker, Character target)
    {
        /* Combat checks such as */

        if (attacker.CurrentHealth <= 0 || target.CurrentHealth <= 0)
        {
            attacker.Combat.Reset();

            return false;
        }

        if (attacker is Player pl)
        {
            if (pl.UsingBow)
            {
                switch (pl.Equipment.GetItem(EquipmentSlot.Ammo).ItemId)
                {
                    case 882:
                        pl.Projectile = Projectile.BRONZE_ARROW;
                        break;
                    case 884:
                        pl.Projectile = Projectile.IRON_ARROW;
                        break;
                    case 886:
                        pl.Projectile = Projectile.STEEL_ARROW;
                        break;
                    case 888:
                        pl.Projectile = Projectile.MITHRIL_ARROW;
                        break;
                    case 890:
                        pl.Projectile = Projectile.ADAMANT_ARROW;
                        break;
                    case 892:
                        pl.Projectile = Projectile.RUNE_ARROW;
                        break;
                    default:
                        pl.PacketSender.SendMessage("You don't have any arrows.");
                        attacker.Combat.Reset();
                        return false;
                }
            }
        }

        return true;
    }

    public static bool IsAttacked(Character character)
    {
        return character.Combat.Attacker != null;
    }
}