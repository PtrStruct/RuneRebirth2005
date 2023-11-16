using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Entities.Combat;

public class CombatManager
{
    public static void Invoke()
    {
        foreach (var player in Server.Players)
        {
            player.PlayerMeleeCombat.Attack();
        }

        foreach (var npc in NPCManager.WorldNPCs)
        {
            npc.Attack();
        }

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