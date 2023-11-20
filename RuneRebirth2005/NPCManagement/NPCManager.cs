using System.Collections.Frozen;
using Newtonsoft.Json;
using RuneRebirth2005.Entities;

namespace RuneRebirth2005.NPCManagement;

public class NPCManager
{
    static Dictionary<int, NPC> mobs = new();

    public static void Load()
    {
        var npcDefs = File.ReadAllText("../../../Data/NPCDefinitions.json");
        var npcSpawns = File.ReadAllText("../../../Data/NPCSpawns.json");

        var _npcDefinitions = JsonConvert.DeserializeObject<List<NPCDefinition>>(npcDefs).OrderBy(o => o.Id).ToList();
        var spawns = JsonConvert.DeserializeObject<List<NPCSpawn>>(npcSpawns).OrderBy(o => o.Id).ToList();

        for (int i = 0; i < spawns.Count; i++)
        {
            var spawn = spawns[i];
            var npc = _npcDefinitions[i];
            LoadNewNPC(spawn, npc, i);
        }

        Server.NPCs = mobs.ToFrozenDictionary();
    }

    private static void LoadNewNPC(NPCSpawn npcSpawn, NPCDefinition npc, int index)
    {
        if (npc == null) return;
        if (npc.Name == null)
            return;

        var mob = new NPC();
        mob.Index = index;
        mob.Size = npc.Size;
        mob.Name = npc.Name;
        mob.ModelId = npc.Id;
        mob.Stationary = npcSpawn.Walk == 1;
        mob.CombatLevel = npc.Combat;
        mob.FallAnimation = npc.DeathAnim;
        mob.AttackSpeed = npc.AttackSpeed;
        mob.BlockAnimation = npc.DefenceAnim;
        mob.AttackAnimation = npc.AttackAnim;
        mob.SpawnLocation = new Location(npcSpawn.X, npcSpawn.Y);
        mob.Location = new Location(npcSpawn.X, npcSpawn.Y);
        mob.CurrentHealth = npc.Hitpoints == 0 ? 1 : npc.Hitpoints;
        mob.MaxHealth = npc.Hitpoints == 0 ? 1 : npc.Hitpoints;

        SetFaceBasedOnWalk(mob, npcSpawn.Walk);
        mobs[index] = mob;
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