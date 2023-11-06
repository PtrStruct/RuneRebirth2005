namespace RuneRebirth2005.Entities;

public class PlayerSkills
{
    public enum Skill
    {
        Attack,
        Defence,
        Strength,
        Hitpoints,
        Ranged,
        Prayer,
        Magic,
        Cooking,
        Woodcutting,
        Fletching,
        Fishing,
        Firemaking,
        Crafting,
        Smithing,
        Mining,
        Herblore,
        Agility,
        Thieving,
        Slayer,
        Farming,
        Runecrafting,
        Hunter
    }

    public int[] Levels { get; set; }

    public PlayerSkills()
    {
        Levels = new int[Enum.GetNames(typeof(Skill)).Length];
        for (int i = 0; i < Levels.Length; i++)
        {
            Levels[i] = 1;
        }
        
        Levels[(int)Skill.Hitpoints] = 10;
    }
}