using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Entities.Combat;

public class CombatManager
{
    public static void Invoke()
    {
        foreach (var player in Server.Players)
        {
            player.MeleeCombat.CalculateAttack();
        }

        foreach (var npc in NPCManager.WorldNPCs)
        {
            npc.MeleeCombat.CalculateAttack();
        }
        
        foreach (var npc in NPCManager.WorldNPCs)
        {
            npc.MeleeCombat.DecideCombatAnimation();
        }

        foreach (var player in Server.Players)
        {
            player.MeleeCombat.DecideCombatAnimation();
        }
    }
    
}