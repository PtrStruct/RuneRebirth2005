using Newtonsoft.Json;
using RuneRebirth2005.Entities;

namespace RuneRebirth2005.NPCManagement;

public class NPCManager
{
    public static List<NPCDefinition> _npcDefinitions = new();
    public static List<NPC> WorldNPCs { get; set; } = new();

    public NPCManager()
    {
        
    }

    public static void Load()
    {
        // WorldNPCs.Add(new NPC
        // {
        //     Index = 0,
        //     Location = new Location(3198, 3200),
        //     ModelId = 1769,
        //     Alive = true,
        //     IsUpdateRequired = true
        // });
        //
        // WorldNPCs.Add(new NPC
        // {
        //     Index = 1,
        //     Location = new Location(3197, 3200),
        //     ModelId = 1769,
        //     Alive = true,
        //     IsUpdateRequired = true
        // });
        
        var npcDefs = File.ReadAllText("../../../Data/NPCDefinitions.json");
        var npcSpawns = File.ReadAllText("../../../Data/NPCSpawns.json");
        
        _npcDefinitions = JsonConvert.DeserializeObject<List<NPCDefinition>>(npcDefs).OrderBy(o => o.Id).ToList();
        var NPCs = JsonConvert.DeserializeObject<List<NPCSpawn>>(npcSpawns).OrderBy(o => o.Id).ToList();
        
        
        for (int i = 0; i < NPCs.Count; i++)
        {
            var npc = NPCs[i];
            LoadNewNPC(npc, i);
        }
    }

    private static void LoadNewNPC(NPCSpawn npcSpawn, int i)
    {
        var npcDef = _npcDefinitions.FirstOrDefault(x => x.Id == npcSpawn.Id);
        if (npcDef == null) return;

        if (npcDef.Name == null)
        {
            return;
        }

        var npc = new NPC
        {
            Index = i,
            ModelId = npcDef.Id,
            Name = npcDef.Name,
            CombatLevel = npcDef.Combat,
            SpawnLocation = new Location(npcSpawn.X, npcSpawn.Y),
            Location = new Location(npcSpawn.X, npcSpawn.Y),
            CanWalk = npcSpawn.Walk == 1,
            Size = npcDef.Size,
            NeedsPlacement = true,
            Health = npcDef.Hitpoints == 0 ? 1 : npcDef.Hitpoints
        };

        SetFaceBasedOnWalk(npc, npcSpawn.Walk);

        WorldNPCs.Add(npc);
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