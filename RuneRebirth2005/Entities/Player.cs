using System.Text.Json;
using RuneRebirth2005.ClientManagement;
using RuneRebirth2005.Network;
using Serilog;

namespace RuneRebirth2005.Entities;

public class Player : Client, IEntity
{
    public PacketHandler PacketHandler { get; set; }
    public PacketStore PacketStore { get; set; } = new();
    public bool DidTeleportOrSpawn { get; set; }
    public bool IsUpdateRequired { get; set; }
    public PlayerUpdateFlags Flags { get; set; } = PlayerUpdateFlags.None;

    public PlayerData Data { get; set; } = new();

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

    public class PlayerData
    {
        public int Gender { get; set; }
        public int HeadIcon { get; set; }
        public int CombatLevel { get; set; }
        public int TotalLevel { get; set; }
        public PlayerColors Colors { get; set; } = new();
        public PlayerEquipment Equipment { get; set; } = new();
        public PlayerSkills PlayerSkills { get; set; } = new();
        public PlayerAppearance Appearance { get; set; } = new();
        public PlayerAppearance PlayerAppearance { get; set; } = new();
        public MovementAnimations MovementAnimations { get; set; } = new();
        public Location Location { get; set; } = new(3200, 3200);
    }

    public void SavePlayer()
    {
        // Get the directory path
        var directoryPath = "Data/Characters";
        var filePath = $"{directoryPath}/{Username}.json";

        // Ensure the directory exists
        Directory.CreateDirectory(directoryPath);

        // Save the file to the directory
        using FileStream createStream = File.Create(filePath);
        JsonSerializer.Serialize(createStream, Data, new JsonSerializerOptions() { WriteIndented = true });
        Log.Information($"Saving player data for: {Username}.");
    }

    public void LoadPlayer()
    {
        var directoryPath = "Data/Characters";
        var filePath = $"{directoryPath}/{Username}.json";

        if (File.Exists(filePath))
        {
            using FileStream openStream = File.OpenRead(filePath);
            Data = JsonSerializer.Deserialize<PlayerData>(openStream);
            Log.Information($"Loaded player data for: {Username}.");
        }
        else
        {
            SavePlayer();
        }
    }
}