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
        
        /* Only do this check if we're not in multi */
        if (IsAttacked(attacker) && attacker.Combat.Attacker != target)
        {
            if (attacker is Player player)
            {
                player.PacketSender.SendMessage("You are already under attack!");
            }

            /* Reset combat */
            attacker.Combat.Reset();
            return false;
        }

        if (IsAttacked(target) && target.Combat.Attacker != attacker)
        {
            if (attacker is Player player)
            {
                player.PacketSender.SendMessage("They are already under attack!");
            }

            /* Reset combat */
            attacker.Combat.Reset();
            return false;
        }

        return true;
    }

    public static bool IsAttacked(Character character)
    {
        return character.Combat.Attacker != null;
    }
}