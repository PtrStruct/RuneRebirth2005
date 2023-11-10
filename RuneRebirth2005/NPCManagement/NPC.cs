using RuneRebirth2005.Entities;

namespace RuneRebirth2005.NPCManagement;

public class NPC : IEntity
{
    public int Index { get; set; }
    public string Name { get; set; }
    public int ModelId { get; set; }
    public Location SpawnLocation { get; set; }
    public int SpawnRadius { get; set; } = 3;
    public Location Location { get; set; }
    public int HeadIcon { get; set; }
    public bool IsUpdateRequired { get; set; }
    public bool NeedsPlacement { get; set; } = true;
    public NPCUpdateFlags Flags { get; set; }
    public int Size { get; set; } = -1;
    public int CombatLevel { get; set; } = 1;
    public int Health { get; set; } = 25;
    public bool CanWalk { get; set; }
    public Face Face { get; set; }
    public bool Alive { get; set; } = true;
    public int AnimationId { get; set; } = -1;
    public int AttackAnimation { get; set; } = -1;
    public int BlockAnimation { get; set; } = -1;
    public int FallAnimation { get; set; } = -1;
}

[Flags]
public enum NPCUpdateFlags
{
    None = 0,
    Animation = 0x10,
    SingleHit = 0x8,
    InteractingEntity = 0x20,
    ForceChat = 0x1,
    Graphics = 0x80,
    DoubleHit = 0x40,
    Transform = 0x2,
    Face = 0x4
}