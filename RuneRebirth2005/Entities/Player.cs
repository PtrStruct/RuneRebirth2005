using RuneRebirth2005.ClientManagement;
using RuneRebirth2005.Network;

namespace RuneRebirth2005.Entities;

public class Player : Client, IEntity
{
    public PacketHandler PacketHandler { get; set; }
    public PacketStore PacketStore { get; set; } = new();
    public Location Location { get; set; } = new(3200, 3200);
    public PlayerAppearance PlayerAppearance { get; set; }
    public bool DidTeleportOrSpawn { get; set; }
    public bool IsUpdateRequired { get; set; }
    public PlayerUpdateFlags Flags { get; set; } = PlayerUpdateFlags.None;
    public int Gender { get; set; }
    public int HeadIcon { get; set; }
    public PlayerAppearance Appearance { get; set; } = new();
    public PlayerEquipment Equipment { get; set; } = new();
    public PlayerColors Colors { get; set; } = new();
    public MovementAnimations MovementAnimations { get; set; } = new();
    public int CombatLevel { get; set; }
    public int TotalLevel { get; set; }

    public Player()
    {
        Index = -1;
        PacketHandler = new PacketHandler(this);
        
    }

    public void Reset()
    {
        Flags |= PlayerUpdateFlags.None;
        IsUpdateRequired = false;
        DidTeleportOrSpawn = false;
    }
}