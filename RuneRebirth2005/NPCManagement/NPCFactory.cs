using RuneRebirth2005.Entities;
using RuneRebirth2005.NPCManagement.Bosses;

namespace RuneRebirth2005.NPCManagement;

public static class NPCFactory
{
    private static readonly Dictionary<int, Func<INPC>> modelIdToNPCFactory = new()
    {
        { 10, () => new CasterNPC() },
        { 13, () => new CasterNPC() },
        { 27, () => new CasterNPC() },
        { 66, () => new CasterNPC() },
        { 67, () => new CasterNPC() },
        { 68, () => new CasterNPC() },
        { 168, () => new CasterNPC() },
        { 169, () => new CasterNPC() },
        { 172, () => new CasterNPC() },
        { 174, () => new CasterNPC() },
        { 277, () => new CasterNPC() },
        { 479, () => new CasterNPC() },
        { 480, () => new CasterNPC() },
        { 678, () => new CasterNPC() },
        { 688, () => new CasterNPC() },
        { 689, () => new CasterNPC() },
        { 690, () => new CasterNPC() },
        { 691, () => new CasterNPC() },
        { 907, () => new CasterNPC() },
        { 908, () => new CasterNPC() },
        { 909, () => new CasterNPC() },
        { 910, () => new CasterNPC() },
        { 911, () => new CasterNPC() },
        { 912, () => new CasterNPC() },
        { 913, () => new CasterNPC() },
        { 914, () => new CasterNPC() },
        { 2025, () => new CasterNPC() },
        { 2028, () => new CasterNPC() },
        { 2463, () => new CasterNPC() },
        { 2464, () => new CasterNPC() },
        { 2465, () => new CasterNPC() },
        { 2466, () => new CasterNPC() },
        { 2467, () => new CasterNPC() },
        { 2468, () => new CasterNPC() },
        { 2558, () => new CasterNPC() },
        { 2559, () => new CasterNPC() },
        { 2560, () => new CasterNPC() },
        { 2564, () => new CasterNPC() },
        { 2565, () => new CasterNPC() },
        { 2631, () => new CasterNPC() },

        /* Bosses */
        { 50, () => new KingBlackDragon() }
    };

    public static INPC CreateNPC(NPCDefinition definition, NPCSpawn spawn, int index)
    {
        var npc = modelIdToNPCFactory.ContainsKey(definition.Id)
            ? modelIdToNPCFactory[definition.Id]()
            : new MeleeNPC();

        npc.Index = index;
        npc.ShouldRender = true;
        npc.Size = definition.Size;
        npc.Name = definition.Name;
        npc.ModelId = definition.Id;
        npc.Stationary = spawn.Walk == 1;
        npc.CombatLevel = definition.Combat;
        npc.FallAnimation = definition.DeathAnim;
        npc.AttackSpeed = definition.AttackSpeed;
        npc.BlockAnimation = definition.DefenceAnim;
        npc.AttackAnimation = definition.AttackAnim;
        npc.SpawnLocation = new Location(spawn.X, spawn.Y);
        npc.CurrentLocation = new Location(spawn.X, spawn.Y);
        npc.CurrentHealth = definition.Hitpoints == 0 ? 1 : definition.Hitpoints;
        npc.MaxHealth = definition.Hitpoints == 0 ? 1 : definition.Hitpoints;

        return npc;
    }
}