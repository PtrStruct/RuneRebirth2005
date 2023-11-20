using RuneRebirth2005.NPCManagement;
using RuneRebirth2005.Entities.Fighting;

namespace RuneRebirth2005.Entities;

public interface IEntity
{
    public int Index { get; set; }
    public IEntity InteractingEntity { get; set; }
    public Face Face { get; set; }
    public Location Location { get; set; }
    public int Size { get; set; }
    public int CurrentHealth { get; set; }
    public bool IsUpdateRequired { get; set; }

    public int CurrentAnimation { get; set; }
    public int AttackAnimation { get; set; }
    public int BlockAnimation { get; set; }
    public int FallAnimation { get; set; }
}

public abstract class Character : IEntity
{
    public abstract int Index { get; set; }
    public abstract IEntity InteractingEntity { get; set; }
    public abstract Face Face { get; set; }
    public abstract Location Location { get; set; }
    public abstract int Size { get; set; }
    public abstract int CurrentHealth { get; set; }
    public abstract bool IsUpdateRequired { get; set; }
    public abstract int CurrentAnimation { get; set; }
    public abstract int AttackAnimation { get; set; }
    public abstract int BlockAnimation { get; set; }
    public abstract int FallAnimation { get; set; }
    public abstract Fighting.Combat Combat { get; set; }
    public abstract int AttackSpeed { get; set; }

    public abstract void SetInteractionEntity(IEntity entity);
    public abstract void PerformAnimation(int animationId);
}