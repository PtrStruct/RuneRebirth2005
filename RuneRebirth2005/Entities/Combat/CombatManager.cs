using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Entities.Combat;

public class CombatManager
{
    public static void Invoke()
    {
        /* If HP <= 0 perform fall animation etc */

        // foreach (var player in Server.Players)
        // {
        //     player.PlayerMeleeCombat.Attack();
        // }

        foreach (var npc in NPCManager.WorldNPCs)
        {
            if (npc.Index == -1) return;
            npc.EndCombatCheck();
        }
        
        /* Attack */
        
        foreach (var player in Server.Players)
        {
            player.PlayerMeleeCombat.Attack();
        }

        foreach (var npc in NPCManager.WorldNPCs)
        {
            npc.Attack();
        }

        /* Animations */
        
        foreach (var npc in NPCManager.WorldNPCs)
        {
            npc.SetCombatAnimation();
        }

        foreach (var player in Server.Players)
        {
            player.PlayerMeleeCombat.SetCombatAnimation();
        }
    }
}