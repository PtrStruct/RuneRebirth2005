using RuneRebirth2005.Entities;
using RuneRebirth2005.World.Clipping;

namespace RuneRebirth2005.Fighting;

public class Combat
{
    public Character Character { get; set; }
    public Character Target { get; set; }
    public Character Attacker { get; set; }
    private int _attackTimer = 0;
    public HitQueue HitQueue { get; set; }

    public bool PerformedHit { get; set; }
    public bool WasHit { get; set; }


    public Combat(Character character)
    {
        Character = character;
        HitQueue = new HitQueue();
    }

    public void Process()
    {
        /* Build Damage and Perform Animation */
        HitQueue.Process(Character);


        if (_attackTimer > 0)
            _attackTimer--;


        /* Add Damage (Display hit splat and lower health) */
        DoCombat();
    }

    public void PerformAnimation()
    {
        

        if (Character.Combat.Target?.CurrentHealth <= 0)
        {
            // Character.PerformAnimation(Character.FallAnimation);
            if (Character is Player player)
            {
                player.PacketSender.SendMessage("You've defeated your enemy!");
                // player.Combat.Attacker = null;
                player.Combat.Reset();
            }

            Reset();
            // Character.Combat.Attacker.Combat.Reset();
        }

        if (!PerformedHit && WasHit)
        {
            Character.PerformAnimation(Character.BlockAnimation);
        }
        else if (PerformedHit)
        {
            Character.PerformAnimation(Character.AttackAnimation);
        }
        
         if (Character.CurrentHealth <= 0)
         {
             Character.PerformAnimation(Character.FallAnimation);
             Reset();
             // Character.Combat.Attacker.Combat.Reset();
        
             if (Character is Player player)
             {
                 DelayedTaskHandler.RegisterTask(new DelayedAttackTask
                 {
                     RemainingTicks = 4,
                     Task = () => { player.Respawn(); }
                 });
             }
        
             if (Character is NPC npc)
             {
                 DelayedTaskHandler.RegisterTask(new DelayedAttackTask
                 {
                     RemainingTicks = 4,
                     Task = () => { npc.Respawn(); }
                 });
             }
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
        //Target = target;
        
        Character.MovementHandler.Reset();
        Character.MovementHandler.FollowCharacter = target;
        Character.SetInteractionEntity(target);
    }

    private void DoCombat()
    {
        if (Target != null)
        {
            if (_attackTimer <= 0)
            {
                /* Perform Animation */

                /* Check if can combat */
                if (CombatHelper.CanAttack(Character, Target))
                {
                    Character.Combat.PerformedHit = true;
                    Target.Combat.WasHit = true;

                    Target.Combat.HitQueue.AddHit(new CombatHit
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

    public void Reset()
    {
        if (Target != null)
        {
            Character.SetInteractionEntity(null);
            Attacker = null;
            Target = null;
        }
    }
}