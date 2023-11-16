using Newtonsoft.Json;
using RuneRebirth2005.Entities;

namespace RuneRebirth2005.NPCManagement;

public class NPCManager
{
    public static List<NPCDefinition> _npcDefinitions = new();
    public static List<INPC> WorldNPCs { get; set; } = new();

    public NPCManager()
    {
    }

    public static void Load()
    {
        // WorldNPCs.Add(new NPC
        // {
        //     Index = 0,
        //     Location = new Location(3193, 3200),
        //     ModelId = 50,
        //     Alive = true,
        //     IsUpdateRequired = true
        // });
        //
        // WorldNPCs.Add(new NPC
        // {
        //     Index = 1,
        //     Location = new Location(3199, 3200),
        //     ModelId = 1769,
        //     Alive = true,
        //     IsUpdateRequired = true
        // });
        //
        // WorldNPCs.Add(new NPC
        // {
        //     Index = 2,
        //     Location = new Location(3201, 3200),
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
            return;

        var npc = NPCFactory.CreateNPC(npcDef, npcSpawn, i);

        SetFaceBasedOnWalk(npc, npcSpawn.Walk);

        WorldNPCs.Add(npc);
    }

    private static void SetFaceBasedOnWalk(INPC npc, int walkValue)
    {
        switch (walkValue)
        {
            case 2:
                npc.Face = new Face(npc.CurrentLocation.X, npc.CurrentLocation.Y + 1);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            case 3:
                npc.Face = new Face(npc.CurrentLocation.X, npc.CurrentLocation.Y - 1);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            case 4:
                npc.Face = new Face(npc.CurrentLocation.X + 1, npc.CurrentLocation.Y);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            case 5:
                npc.Face = new Face(npc.CurrentLocation.X - 1, npc.CurrentLocation.Y);
                npc.Flags |= NPCUpdateFlags.Face;
                break;
            default:
                npc.Face = new Face(npc.CurrentLocation.X, npc.CurrentLocation.Y);
                break;
        }
    }
}