using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Fighting;

public interface ICombatMethod
{
    bool CanAttack(Character attacker, Character target);
    bool PreQueueAdd(Character attacker, Character target);
    public int AttackSpeed { get; set; }
    public int AttackDistance { get; set; }
}