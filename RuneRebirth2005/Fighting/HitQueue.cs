using RuneRebirth2005.Entities;
using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Fighting;

public class HitQueue
{
    private List<CombatHit> _pendingHits = new List<CombatHit>();

    public void Process(Character character)
    {
        if (character.CurrentHealth <= 0)
        {
            _pendingHits.Clear();
        }

        /* Extract this to another function */
        for (int i = 0; i < _pendingHits.Count; i++)
        {
            var combatHit = _pendingHits[i];
            Character attacker = combatHit.Attacker;
            Character target = combatHit.Target;

            // attacker.PerformAnimation(attacker.AttackAnimation);
            //target.PerformAnimation(target.BlockAnimation);
            // target.Combat.WasHit = true;

            if (target.Combat.Target == null)
            {
                DelayedTaskHandler.RegisterTask(new DelayedAttackTask
                {
                    RemainingTicks = 1,
                    Task = () => { target.Combat.Attack(attacker); }
                });
            }

            _pendingHits.Add(combatHit);

            /* Set Under Attack */
            target.Combat.Attacker = attacker;

            _pendingHits.Remove(combatHit);
        }

        if (_pendingHits.Count > 0)
        {
            var hit = _pendingHits.First();
            character.Combat.Target.PrimaryDamage = _pendingHits.First();
            
            if (character.Combat.Target is Player player)
            {
                player.Flags |= PlayerUpdateFlags.SingleHit;
            }
            else if (character.Combat.Target is NPC npc)
            {
                npc.Flags |= NPCUpdateFlags.SingleHit;
            }

            character.Combat.Target.IsUpdateRequired = true;
            _pendingHits.Remove(hit);
        }
    }

    public void AddHit(CombatHit hit)
    {
        _pendingHits.Add(hit);
    }
}

public class CombatHit
{
    public Character Attacker { get; set; }
    public Character Target { get; set; }
    public int Damage { get; set; }
    public int HitType { get; set; }
}