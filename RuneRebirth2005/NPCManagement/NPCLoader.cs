using System.Collections.Frozen;
using System.Text.Json;
using System.Text.Json.Serialization;
using RuneRebirth2005.Entities;
using RuneRebirth2005.World.Clipping;

namespace RuneRebirth2005.NPCManagement;

public class NPCLoader
{
    public static FrozenDictionary<int, Definition> Definitions { get; set; }
    public static List<Spawn> Spawns { get; set; }

    public static void Load()
    {
        var npcDefs = File.ReadAllText("../../../Data/Defs.json");
        var npcSpawns = File.ReadAllText("../../../Data/Spawns.json");
        Definitions = JsonSerializer.Deserialize<List<Definition>>(npcDefs).ToDictionary(item => item.Id, item => item).ToFrozenDictionary();
        Spawns = JsonSerializer.Deserialize<List<Spawn>>(npcSpawns);

        for (int i = 0; i < Spawns.Count; i++)
        {
            if (Definitions.TryGetValue(Spawns[i].Id, out var def))
            {
                var npc = new NPC
                {
                    Index = i,
                    Size = def.Size,
                    Name = def.Name,
                    ModelId = def.Id,
                    Stationary = def.Walk != 1,
                    CombatLevel = def.Combat,
                    FallAnimation = def.DeathAnim,
                    AttackSpeed = def.AttackSpeed,
                    BlockAnimation = def.DefenceAnim,
                    AttackAnimation = def.AttackAnim,
                    SpawnLocation = new Location(Spawns[i].X, Spawns[i].Y),
                    Location = new Location(Spawns[i].X, Spawns[i].Y),
                    CurrentHealth = def.Hitpoints == 0 ? 1 : def.Hitpoints,
                    MaxHealth = def.Hitpoints == 0 ? 1 : def.Hitpoints,
                    IsUpdateRequired = true,
                    DumbPathFinder = new NPCDumbPathFinder(),
                };

                SetFaceBasedOnWalk(npc, def.Walk);
                Server.NPCs.Add(npc);
            }
        }
    }

    private static void SetFaceBasedOnWalk(NPC npc, int walkValue)
    {
        switch (walkValue)
        {
            case 2:
                npc.Face = new Face(npc.Location.X, npc.Location.Y + 1);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            case 3:
                npc.Face = new Face(npc.Location.X, npc.Location.Y - 1);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            case 4:
                npc.Face = new Face(npc.Location.X + 1, npc.Location.Y);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            case 5:
                npc.Face = new Face(npc.Location.X - 1, npc.Location.Y);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            default:
                npc.Face = new Face(npc.Location.X, npc.Location.Y);
                break;
        }
    }
}

public class Definition
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("examine")] public string Examine { get; set; }

    [JsonPropertyName("combat")] public int Combat { get; set; }

    [JsonPropertyName("size")] public int Size { get; set; }

    [JsonPropertyName("attackable")] public bool Attackable { get; set; }

    [JsonPropertyName("aggressive")] public bool Aggressive { get; set; }

    [JsonPropertyName("retreats")] public bool Retreats { get; set; }

    [JsonPropertyName("poisonous")] public bool Poisonous { get; set; }

    [JsonPropertyName("respawn")] public int Respawn { get; set; }

    [JsonPropertyName("maxHit")] public int MaxHit { get; set; }

    [JsonPropertyName("hitpoints")] public int Hitpoints { get; set; }

    [JsonPropertyName("attackSpeed")] public int AttackSpeed { get; set; }

    [JsonPropertyName("attackAnim")] public int AttackAnim { get; set; }

    [JsonPropertyName("defenceAnim")] public int DefenceAnim { get; set; }

    [JsonPropertyName("deathAnim")] public int DeathAnim { get; set; }

    [JsonPropertyName("attackBonus")] public int? AttackBonus { get; set; }

    [JsonPropertyName("defenceMelee")] public int DefenceMelee { get; set; }

    [JsonPropertyName("defenceRange")] public int DefenceRange { get; set; }

    [JsonPropertyName("defenceMage")] public int DefenceMage { get; set; }

    [JsonPropertyName("strength")] public int Strength { get; set; }

    [JsonPropertyName("attack")] public int Attack { get; set; }

    [JsonPropertyName("walk")] public int Walk { get; set; }
}

public class Spawn
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("x")] public int X { get; set; }

    [JsonPropertyName("y")] public int Y { get; set; }

    [JsonPropertyName("height")] public int Height { get; set; }
}