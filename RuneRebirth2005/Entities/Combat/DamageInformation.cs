namespace RuneRebirth2005.Entities.Combat;

public class DamageInformation
{
    public int Amount { get; set; }
    public IEntity HitBy { get; set; }
    public bool HasBeenHit { get; set; }
    public DamageType DamageType { get; set; }
}

public enum DamageType
{
    Block,
    Damage,
    Poison,
    Desease,
    DeseaseAlternative
}