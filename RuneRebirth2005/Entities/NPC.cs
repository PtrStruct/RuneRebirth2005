using RuneRebirth2005.Fighting;
using RuneRebirth2005.Handlers;
using RuneRebirth2005.NPCManagement;
using RuneRebirth2005.World.Clipping;

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
    public override MovementHandler MovementHandler { get; set; }

    public int MaxHealth { get; set; }
    public override int CurrentHealth { get; set; }
    public override int Size { get; set; }

    // locations
    public Location SpawnLocation { get; set; }
    public override Location Location { get; set; }

    // interaction
    public override IEntity InteractingEntity { get; set; }
    public override Face Face { get; set; }
    public override bool IsUpdateRequired { get; set; }

    // combat and actions
    public override Combat Combat { get; set; }
    public override int FallAnimation { get; set; }
    public override int BlockAnimation { get; set; }
    public override int CurrentAnimation { get; set; }
    public override int AttackAnimation { get; set; }
    public override CombatHit PrimaryDamage { get; set; }
    public override CombatHit SecondaryDamage { get; set; }
    public NPCDumbPathFinder DumbPathFinder { get; set; }

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
        MovementHandler.PrimaryDirection = -1;
        MovementHandler.SecondaryDirection = -1;
        Flags = NPCUpdateFlags.None;
    }
    
    public void Respawn()
    {
        // Combat.Reset();
        CurrentHealth = 10;
        MaxHealth = 10;
        PerformAnimation(-1);
        IsUpdateRequired = true;
        Flags |= NPCUpdateFlags.Animation;
    }
}