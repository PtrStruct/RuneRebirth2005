using RuneRebirth2005.Entities.Combat;

namespace RuneRebirth2005.Entities;

public interface IEntity
{
    public int Index { get; set; }
    public IEntity CombatFocus { get; set; }
    public Weapon Weapon { get; set; }
    public DamageInformation RecentDamageInformation { get; set; }
    public int CurrentAnimation { get; set; }
    public int AttackAnimation { get; set; }
    public int BlockAnimation { get; set; }
    public int FallAnimation { get; set; }
    public bool IsUpdateRequired { get; set; }
}