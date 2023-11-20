using RuneRebirth2005.Entities;

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

            _pendingHits.Remove(combatHit);

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