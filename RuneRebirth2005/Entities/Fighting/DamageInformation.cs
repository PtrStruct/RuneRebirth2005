using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Entities.Fighting;

public class DamageInformation
{
    public int Amount { get; set; }
    public Character HitBy { get; set; }
    public Character HitByNPC { get; set; }
    public bool HasBeenHit { get; set; }
    public DamageType DamageType { get; set; }

    public void Reset()
    {
        Amount = -1;
        HitBy = null;
        HasBeenHit = false;
    }
    
}

public enum DamageType
{
    Block,
    Damage,
    Poison,
    Desease,
    DeseaseAlternative
}