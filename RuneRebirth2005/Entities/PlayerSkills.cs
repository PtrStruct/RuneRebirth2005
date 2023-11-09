using Serilog;

namespace RuneRebirth2005.Entities;

public class PlayerSkills
{
    public Dictionary<SkillEnum, Skill> Skills { get; set; }

    public PlayerSkills()
    {
        if (Skills == null)
        {
            Skills = new Dictionary<SkillEnum, Skill>();
            foreach (SkillEnum skillEnum in Enum.GetValues(typeof(SkillEnum)))
            {
                Skills[skillEnum] = new Skill { Level = 1, Experience = 0 };
            }

            Skills[SkillEnum.Hitpoints].Level = 10;
            Skills[SkillEnum.Hitpoints].Experience = 1154;
        }
    }

    public Skill GetSkill(SkillEnum skill)
    {
        return Skills.TryGetValue(skill, out var skillValue)
            ? skillValue
            : null;
    }

    public void SetSkill(SkillEnum skillType, int level)
    {
        if (Skills.ContainsKey(skillType))
        {
            Skills[skillType].Level = level;
        }
        else
        {
            Log.Warning($"Skill {skillType} does not exist.");
        }
    }

    public void BoostSkill(SkillEnum skillType, int level)
    {
        if (Skills.ContainsKey(skillType))
            Skills[skillType].Level = level;
        else
            Log.Warning($"Skill {skillType} does not exist.");
    }
}

public class Skill
{
    private int level;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            Experience = GetXPForLevel(value);
        }
    }

    public int Experience { get; set; }

    private int GetXPForLevel(int level)
    {
        double points = 0;
        int output = 0;

        for (int lvl = 1; lvl <= level; lvl++)
        {
            points += (int)Math.Floor(lvl + 300.0 * Math.Pow(2.0, lvl / 7.0));
            if (lvl >= level)
                return output;
            output = (int)Math.Floor(points / 4);
        }

        return 0;
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