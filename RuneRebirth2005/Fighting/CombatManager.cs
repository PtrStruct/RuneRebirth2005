using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Fighting;

public class CombatManager
{
    public static void Invoke()
    {
        /* If HP <= 0 perform fall animation etc */
        foreach (var npc in Server.NPCs)
        {
            // if (npc.Index == -1) return;
            // npc.EndCombatCheck();
        }
        
        /* Attack */
        foreach (var player in Server.Players)
        {
            // player.PlayerMeleeCombat.Attack();
        }

        foreach (var npc in Server.NPCs)
        {
            // npc.Attack();
        }

        /* Animations */
        foreach (var npc in Server.NPCs)
        {
            // npc.SetCombatAnimation();
        }

        foreach (var player in Server.Players)
        {
            // player.PlayerMeleeCombat.SetCombatAnimation();
        }
    }
}