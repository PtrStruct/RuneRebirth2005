using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Fighting;

public class PlayerCombat : CombatBase
{
    private readonly Player _character;

    public PlayerCombat(Player character) : base(character)
    {
        _character = character;
    }

    public override void DoAttack()
    {
        if (Target != null)
        {
            if (CanReach(Character, Target))
            {
                if (AttackTimer <= 0)
                {
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
                        }, true);
                        AttackTimer = Character.AttackSpeed;
                    }
                }
            }
        }
    }

    private bool CanReach(Character attacker, Character target)
    {
        return attacker.Location.IsWithinDistance(target.Location, 1);
    }

    public override void PerformAnimation()
    {
        if (Character.Combat.Target?.CurrentHealth <= 0)
        {
            var player = Character as Player;
            player.PacketSender.SendMessage("You've defeated your enemy!");
            player.MovementHandler.Reset();
            Reset();
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

            var player = Character as Player;
            DelayedTaskHandler.RegisterTask(new DelayedAttackTask
            {
                RemainingTicks = 4,
                Task = () => { player.Respawn(); }
            });
        }
    }
}