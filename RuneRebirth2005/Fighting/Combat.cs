using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Fighting;

public class Combat
{
    public Character Character { get; set; }
    public Character Target { get; set; }
    private int _attackTimer = 0;
    private HitQueue _hitQueue;

    public bool PerformedHit { get; set; }
    public bool WasHit { get; set; }


    public Combat(Character character)
    {
        Character = character;
        _hitQueue = new HitQueue();
    }

    public void Process()
    {
        /* Build Damage and Perform Animation */
        _hitQueue.Process(Character);


        if (_attackTimer > 0)
            _attackTimer--;


        /* Add Damage (Display hit splat and lower health) */
        DoCombat();
    }

    public void PerformAnimation()
    {
        
         if (!PerformedHit && WasHit)
         {
             Character.PerformAnimation(Character.BlockAnimation);
         }
         else if (PerformedHit)
         {
             Character.PerformAnimation(Character.AttackAnimation);
         }
        
        // if (PerformedHit && WasHit)
        // {
        //     Character.PerformAnimation(Character.AttackAnimation);
        // }
        //
        // if (!PerformedHit && WasHit)
        // {
        //     Character.PerformAnimation(Character.BlockAnimation);
        // }
        // else
        // {
        //     Character.PerformAnimation(-1);
        // }
    }

    public void Attack(Character target)
    {
        Target = target;
        Character.SetInteractionEntity(target);
    }

    private void DoCombat()
    {
        if (Target != null)
        {
            if (_attackTimer <= 0)
            {
                /* Perform Animation */
                //Character.PerformAnimation(Character.AttackAnimation);
                // Target.PerformAnimation(Target.BlockAnimation);

                Character.Combat.PerformedHit = true;
                Target.Combat.WasHit = true;
                
                _hitQueue.AddHit(new CombatHit
                {
                    Damage = 1,
                    HitType = 1,
                    Attacker = Character,
                    Target = Target
                });
                _attackTimer = Character.AttackSpeed;
            }
        }
    }
}