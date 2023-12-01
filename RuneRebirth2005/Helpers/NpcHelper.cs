namespace RuneRebirth2005.Helpers;

public class NpcHelper
{
    public static Dictionary<int, NpcCombat> npcs = new Dictionary<int, NpcCombat>
    {
        // NPCs with AttackType = 2 and ProjectileId = 280
        { 1158, new NpcCombat { Id = 1158, AttackType = 2, ProjectileId = 280, EndGfx = 279 } },

        // NPCs with AttackType = 0
        { 1160, new NpcCombat { Id = 1160, AttackType = 0, ProjectileId = 279, EndGfx = 278 } },
        { 134, new NpcCombat { Id = 134, AttackType = 0 } }, // assuming melee
        { 2561, new NpcCombat { Id = 2561, AttackType = 0 } },
        { 2563, new NpcCombat { Id = 2563, AttackType = 0 } }, // assuming melee
        { 2550, new NpcCombat { Id = 2550, AttackType = 0 } }, // assuming melee
        { 2551, new NpcCombat { Id = 2551, AttackType = 0 } }, // assuming melee

        // NPC's with AttackType = 1
        { 2607, new NpcCombat { Id = 2607, AttackType = 1 } },
        { 2894, new NpcCombat { Id = 2894, ProjectileId = 298, AttackType = 1 } },
        { 2560, new NpcCombat { Id = 2560, ProjectileId = 1190, AttackType = 1 } },
        { 2553, new NpcCombat { Id = 2553, ProjectileId = 1206, AttackType = 1 } },
        { 2565, new NpcCombat { Id = 2565, ProjectileId = 9, AttackType = 1 } },
        { 2881, new NpcCombat { Id = 2881, AttackType = 1, ProjectileId = 298 } },
        { 2028, new NpcCombat { Id = 2028, AttackType = 1, ProjectileId = 27 } },
        { 3762, new NpcCombat { Id = 3762, AttackType = 1 } },

        // NPCs with AttackType = 2
        { 2591, new NpcCombat { Id = 2591, AttackType = 2 } },
        { 172, new NpcCombat { Id = 172, ProjectileId = 97, AttackType = 2, EndGfx = 98 } },
        { 174, new NpcCombat { Id = 174, ProjectileId = 97, AttackType = 2, EndGfx = 98 } },
        { 2892, new NpcCombat { Id = 2892, ProjectileId = 94, AttackType = 2, EndGfx = 95 } },
        { 2559, new NpcCombat { Id = 2559, ProjectileId = 1203, AttackType = 2 } },
        { 2558, new NpcCombat { Id = 2558, AttackType = 2, ProjectileId = 1198 } }, // assuming magic
        { 2562, new NpcCombat { Id = 2562, AttackType = 2, EndGfx = 1224 } }, // assuming magic
        { 2564, new NpcCombat { Id = 2564, ProjectileId = 1203, AttackType = 2 } }, // assuming magic
        { 2552, new NpcCombat { Id = 2552, ProjectileId = 1203, AttackType = 2 } }, // assuming magic
        { 2882, new NpcCombat { Id = 2882, AttackType = 2, ProjectileId = 162, EndGfx = 477 } },
        { 2463, new NpcCombat { Id = 2463, AttackType = 2 } },
        { 3752, new NpcCombat { Id = 3752, AttackType = 2 } },

        // NPCs with AttackType = 2 and partial examples
        { 2025, new NpcCombat { Id = 2025, AttackType = 2, ProjectileId = 162, EndGfx = 163 } }, // Partial example
        { 3200, new NpcCombat { Id = 3200, AttackType = 2, ProjectileId = 554, EndGfx = 555 } }, // Partial example
        { 2745, new NpcCombat { Id = 2745, AttackType = 2, ProjectileId = 448, EndGfx = 157 } }, // Partial example
        { 2743, new NpcCombat { Id = 2743, AttackType = 2, ProjectileId = 445, EndGfx = 446 } },

        // NPCs with AttackType = 3
        { 3068, new NpcCombat { Id = 3068, ProjectileId = 393, AttackType = 3, EndGfx = 430 } },
        { 3590, new NpcCombat { Id = 3590, AttackType = 3 } }, // assuming magic

        // NPCs with multiple cases
        { 50, new NpcCombat { Id = 50, ProjectileId = 393, EndGfx = 430, AttackType = 3 } },
        { 742, new NpcCombat { Id = 742, ProjectileId = 393, EndGfx = 430, AttackType = 3 } },

        // Dragons
        { 5363, new NpcCombat { Id = 5363, AttackType = 3, ProjectileId = 393, EndGfx = 430 } },
        { 53, new NpcCombat { Id = 53, AttackType = 3, ProjectileId = 393, EndGfx = 430 } },
        { 54, new NpcCombat { Id = 54, AttackType = 3, ProjectileId = 393, EndGfx = 430 } },
        { 55, new NpcCombat { Id = 55, AttackType = 3, ProjectileId = 393, EndGfx = 430 } },
        { 941, new NpcCombat { Id = 941, AttackType = 3, ProjectileId = 393, EndGfx = 430 } },
        { 4682, new NpcCombat { Id = 4682, AttackType = 3, ProjectileId = 393, EndGfx = 430 } },
        { 5362, new NpcCombat { Id = 5362, AttackType = 3, ProjectileId = 393, EndGfx = 430 } },
        { 1590, new NpcCombat { Id = 1590, AttackType = 3, ProjectileId = 393, EndGfx = 430 } },
        { 1591, new NpcCombat { Id = 1591, AttackType = 3, ProjectileId = 393, EndGfx = 430 } },
        { 1592, new NpcCombat { Id = 1592, AttackType = 3, ProjectileId = 393, EndGfx = 430 } }
    };
}

public class NpcCombat
{
    public int Id { get; set; }
    public int ProjectileId { get; set; }
    public int AttackType { get; set; }
    public int EndGfx { get; set; }
    public int AttackDistance
    {
        get
        {
            int distanceNeeded = 1;
            switch (AttackType)
            {
                case 1:
                    distanceNeeded += 7;
                    break;
                case 2:
                    distanceNeeded += 9;
                    break;
                default:
                    if (AttackType > 2)
                    {
                        distanceNeeded += 4;
                    }
                    break;
            }
            return distanceNeeded;
        }
    }
}