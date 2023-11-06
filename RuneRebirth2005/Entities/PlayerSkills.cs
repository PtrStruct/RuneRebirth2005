namespace RuneRebirth2005.Entities;

public class PlayerSkills
{
    public int[] Levels { get; set; }
    public int[] Experience { get; set; }
    public PlayerSkills()
    {
        int totalSkills = Enum.GetNames(typeof(SkillEnum)).Length;
        Levels = new int[totalSkills];
        Experience = new int[totalSkills];
        
        for (int i = 0; i < totalSkills; i++)
        {
            Levels[i] = 1;
            Experience[i] = 0; // Initial experience can be set to 0, can be modified according to your game rules.
        }
        
        Levels[(int)SkillEnum.Hitpoints] = 10;
        Experience[(int)SkillEnum.Hitpoints] = 1154;
    }
}

public enum SkillEnum
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