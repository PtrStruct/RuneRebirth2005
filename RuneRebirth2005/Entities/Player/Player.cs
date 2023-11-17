using System.Text.Json;
using RuneRebirth2005.ClientManagement;
using RuneRebirth2005.Entities.Combat;
using RuneRebirth2005.Network;
using RuneRebirth2005.NPCManagement;
using Serilog;

namespace RuneRebirth2005.Entities;

public class Player : Client, IPlayer
{
    public PacketHandler PacketHandler { get; set; }
    public PacketStore PacketStore { get; set; } = new();
    public bool DidTeleportOrSpawn { get; set; }
    public bool IsUpdateRequired { get; set; }
    public CombatType AttackType { get; set; }
    public bool InCombat { get; set; }

    public void Attack()
    {
        throw new NotImplementedException();
    }

    public PlayerUpdateFlags Flags { get; set; } = PlayerUpdateFlags.None;
    public int InteractingEntityId { get; set; } = -1;
    public INPC NPCCombatFocus { get; set; }
    public IPlayer PlayerCombatFocus { get; set; }
    public Weapon Weapon { get; set; }
    public DamageInformation RecentDamageInformation { get; set; } = new();
    public PlayerMeleeCombat PlayerMeleeCombat { get; set; }
    public ICombat Combat { get; set; }
    public int CurrentAnimation { get; set; } = -1;
    public int AttackAnimation { get; set; } = 422;
    public int BlockAnimation { get; set; } = 404;
    public int FallAnimation { get; set; } = -1;


    public Player()
    {
        Index = -1;
        PacketHandler = new PacketHandler(this);
        Weapon = new Weapon
        {
            Speed = 4
        };

        PlayerMeleeCombat = new PlayerMeleeCombat(this);
    }

    public void Reset()
    {
        Flags = PlayerUpdateFlags.None;
        IsUpdateRequired = false;
        DidTeleportOrSpawn = false;
        RecentDamageInformation.HasBeenHit = false;
        PlayerMeleeCombat.PerformedHit = false;
        CurrentAnimation = -1;
    }


    public void SavePlayer()
    {
        // Get the directory path
        var directoryPath = "Data/Characters";
        var filePath = $"{directoryPath}/{Data.Username}.json";

        // Ensure the directory exists
        Directory.CreateDirectory(directoryPath);

        // Save the file to the directory
        using FileStream createStream = File.Create(filePath);
        JsonSerializer.Serialize(createStream, Data, new JsonSerializerOptions() { WriteIndented = true });
        Log.Information($"Saving player data for: {Data.Username}.");
    }

    public void LoadPlayer()
    {
        var directoryPath = "Data/Characters";
        var filePath = $"{directoryPath}/{Data.Username}.json";

        if (!File.Exists(filePath))
            SavePlayer();

        using FileStream openStream = File.OpenRead(filePath);
        Data = JsonSerializer.Deserialize<PlayerData>(openStream);
        Log.Information($"Loaded player data for: {Data.Username}.");
    }

    public void CalculateCombatLevel()
    {
        PlayerSkills skills = new PlayerSkills();

        int mag = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Magic).Experience);
        int ran = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Ranged).Experience);
        int att = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Attack).Experience);
        int str = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Strength).Experience);
        int def = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Defence).Experience);
        int hp = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Hitpoints).Experience);
        int pray = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Prayer).Experience);

        if (ran > att + str)
        {
            Data.CombatLevel = (int)Math.Floor((def * 0.25) + (hp * 0.25) + (pray * 0.125) + (ran * 0.4875));
        }
        else if (mag > att + str)
        {
            Data.CombatLevel = (int)Math.Floor((def * 0.25) + (hp * 0.25) + (pray * 0.125) + (mag * 0.4875));
        }
        else
        {
            Data.CombatLevel =
                (int)Math.Floor((def * 0.25) + (hp * 0.25) + (pray * 0.125) + (att * 0.325) + (str * 0.325));
        }
    }
}