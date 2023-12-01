using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Fighting;

public abstract class CombatBase
{
    public Character Character { get; set; }
    public Character Target { get; set; }
    public Character Attacker { get; set; }
    public int AttackTimer { get; set; }
    public bool PerformedHit { get; set; }
    public bool WasHit { get; set; }
    public HitQueue HitQueue { get; set; } = new();

    //Constructor and common methods like AddHit(), DoCombat(), CanReach() etc.

    protected CombatBase(Character character)
    {
        Character = character;
    }

    public virtual void Process()
    {
        if (Character?.CurrentHealth <= 0 || Target?.CurrentHealth <= 0)
            return;

        HitQueue.Process(Character);

        if (AttackTimer > 0)
            AttackTimer--;

        DoAttack();
    }

    public abstract void DoAttack();

    public abstract void PerformAnimation();

    public void Attack(Character target)
    {
        Target = target;
        Character.MovementHandler.Reset();
        Character.MovementHandler.FollowCharacter = target;
        Character.SetInteractionEntity(target);
    }

    public void Reset()
    {
        if (Target != null)
        {
            Character.SetInteractionEntity(null);
            Attacker = null;
            Target = null;
            Character.MovementHandler.FollowCharacter = null;
        }
    }
}

public enum AttackType
{
    MELEE,
    MAGIC,
    RANGE
}