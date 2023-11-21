using RuneRebirth2005.Fighting;
using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Entities;

public class NPC : Character
{
    public int ModelId { get; set; }
    public string Name { get; set; }
    public override int Index { get; set; }

    // attributes
    public NPCUpdateFlags Flags { get; set; }
    public bool Stationary { get; set; }
    public int CombatLevel { get; set; }
    public override int AttackSpeed { get; set; }

    public int MaxHealth { get; set; }
    public override int CurrentHealth { get; set; }
    public override int Size { get; set; }

    // locations
    public Location SpawnLocation { get; set; }
    public override Location Location { get; set; }
    //public Location Location { get; set; } // this property is commented out, but grouped in location section

    // interaction
    public override IEntity InteractingEntity { get; set; }
    public override Face Face { get; set; }
    public override bool IsUpdateRequired { get; set; }

    // combat and actions
    public override Fighting.Combat Combat { get; set; }
    public override int FallAnimation { get; set; }
    public override int BlockAnimation { get; set; }
    public override int CurrentAnimation { get; set; }
    public override int AttackAnimation { get; set; }
    public override CombatHit PrimaryDamage { get; set; }
    public override CombatHit SecondaryDamage { get; set; }

    public void Process()
    {
        if (Flags != NPCUpdateFlags.None)
            IsUpdateRequired = true;
    }

    public override void SetInteractionEntity(IEntity entity)
    {
        InteractingEntity = entity;
        Flags |= NPCUpdateFlags.InteractingEntity;
    }

    public override void PerformAnimation(int animationId)
    {
        CurrentAnimation = animationId;
        Flags |= NPCUpdateFlags.Animation;
    }

    public void Reset()
    {
        Combat.PerformedHit = false;
        Combat.WasHit = false;
        IsUpdateRequired = false;
        Flags = NPCUpdateFlags.None;
    }
}