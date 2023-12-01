using RuneRebirth2005.Entities;
using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Fighting;

public class HitQueue
{
    List<CombatHit> PerformedHits { get; } = new();
    List<CombatHit> ReceivedHits { get; } = new();

    public void AddHit(CombatHit hit, bool isPerformed)
    {
        if (isPerformed)
        {
            PerformedHits.Add(hit);
        }
        else
        {
            ReceivedHits.Add(hit);
        }
    }

    private void ProcessPerformedHits(Character character)
    {
        // Insert logic for processing performed hits...
        for (int i = 0; i < PerformedHits.Count; i++)
        {
            var combatHit = PerformedHits[i];
            Character attacker = combatHit.Attacker;
            Character target = combatHit.Target;


            if (target.Combat.Target == null)
            {
                DelayedTaskHandler.RegisterTask(new DelayedAttackTask
                {
                    RemainingTicks = 1,
                    Task = () => { target.Combat.Attack(attacker); }
                });
            }

            target.Combat.HitQueue.AddHit(combatHit, false);

            /* Set Under Attack */
            target.Combat.Attacker = attacker;

            PerformedHits.Remove(combatHit);
        }

        PerformedHits.Clear();
    }

    private void ProcessReceivedHits(Character character)
    {
        if (character.CurrentHealth <= 0)
        {
            ReceivedHits.Clear();
            return;
        }

        if (ReceivedHits.Count > 0)
        {
            var hit = ReceivedHits.First();
            hit.Target.PrimaryDamage = hit;
            hit.Target.CurrentHealth -= hit.Damage;

            if (hit.Target is Player player)
            {
                player.Flags |= PlayerUpdateFlags.SingleHit;
            }
            else if (hit.Target is NPC npc)
            {
                npc.Flags |= NPCUpdateFlags.SingleHit;
            }

            hit.Target.IsUpdateRequired = true;
            ReceivedHits.Remove(hit);
        }

        ReceivedHits.Clear();
    }

    public void Process(Character character)
    {
        ProcessPerformedHits(character);
        ProcessReceivedHits(character);
    }
}

public class CombatHit
{
    public Character Attacker { get; set; }
    public Character Target { get; set; }
    public int Damage { get; set; }
    public int HitType { get; set; }
}