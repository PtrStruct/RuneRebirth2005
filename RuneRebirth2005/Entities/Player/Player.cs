using RuneRebirth2005.Fighting;
using RuneRebirth2005.Handlers;
using RuneRebirth2005.Network;
using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Entities;

public class Player : Character
{
    // Identifiers
    public override int Index { get; set; }
    public string Username { get; set; }

    // Authentication
    public string Password { get; set; }

    // General attributes
    public override IEntity InteractingEntity { get; set; }
    public byte Gender { get; set; }
    public byte HeadIcon { get; set; }
    public int CombatLevel { get; set; }
    public int TotalLevel { get; set; }
    public bool IsInventoryUpdate { get; set; }
    public bool IsAppearanceUpdate { get; set; }
    public PlayerUpdateFlags Flags { get; set; }
    public override MovementHandler MovementHandler { get; set; }
    public bool Running { get; set; }

    // Player representations
    public PlayerColors Colors { get; set; } = new();
    public PlayerEquipment Equipment { get; set; } = new();
    public PlayerAppearance Appearance { get; set; } = new();
    public MovementAnimations MovementAnimations { get; set; } = new();
    public PlayerBonuses Bonuses { get; set; } = new();

    // Player skills
    public PlayerSkills PlayerSkills { get; set; } = new();

    // Interactions
    //public IEntity Target { get; set; }
    public override Face Face { get; set; }
    public List<Player> LocalPlayers { get; set; } = new();
    public List<NPC> LocalNPCs { get; set; } = new();

    // Location and status
    public override Location Location { get; set; }

    public override int Size { get; set; } = 1;
    public override int CurrentHealth { get; set; } = 10;
    public override bool IsUpdateRequired { get; set; }
    public bool PlacementOrTeleport { get; set; }

    // Animation and combat
    public override Combat Combat { get; set; }
    public override int AttackSpeed { get; set; } = 5;
    public override void SetInteractionEntity(IEntity entity)
    {
        InteractingEntity = entity;
        Flags |= PlayerUpdateFlags.InteractingEntity;
    }

    public override void PerformAnimation(int animationId)
    {
        CurrentAnimation = animationId;
        Flags |= PlayerUpdateFlags.Animation;
    }

    public override int CurrentAnimation { get; set; }
    public override int AttackAnimation { get; set; } = 422;
    public override int BlockAnimation { get; set; } = 424;
    public override int FallAnimation { get; set; } = 836;
    
    public override CombatHit PrimaryDamage { get; set; }
    public override CombatHit SecondaryDamage { get; set; }

    // Network information
    public PlayerSession PlayerSession { get; }
    public LoginHandler LoginHandler { get; set; }
    public PacketSender PacketSender { get; set; }

    public Player(PlayerSession session)
    {
        PlayerSession = session;
        LoginHandler = new LoginHandler(this);
        PacketSender = new PacketSender(this);
        MovementHandler = new MovementHandler(this);
        
        Location = new Location(3200, 3930); ////3293, 3174 3200, 3930
        Location.Player = this;
        IsUpdateRequired = true;
    }

    public void Process()
    {
        
        if (IsAppearanceUpdate)
            Flags |= PlayerUpdateFlags.Appearance;

        /* Handle Boosted Stats */

        if (Flags != PlayerUpdateFlags.None)
            IsUpdateRequired = true;
    }

    public void SavePlayer()
    {
        // Get the directory path
        // var directoryPath = "Data/Characters";
        // var filePath = $"{directoryPath}/{Data.Username}.json";
        //
        // // Ensure the directory exists
        // Directory.CreateDirectory(directoryPath);
        //
        // // Save the file to the directory
        // using FileStream createStream = File.Create(filePath);
        // JsonSerializer.Serialize(createStream, Data, new JsonSerializerOptions() { WriteIndented = true });
        // Log.Information($"Saving player data for: {Data.Username}.");
    }

    public void LoadPlayer()
    {
        // var directoryPath = "Data/Characters";
        // var filePath = $"{directoryPath}/{Data.Username}.json";
        //
        // if (!File.Exists(filePath))
        //     SavePlayer();
        //
        // using FileStream openStream = File.OpenRead(filePath);
        // Data = JsonSerializer.Deserialize<PlayerData>(openStream);
        // Log.Information($"Loaded player data for: {Data.Username}.");
    }

    public void CalculateCombatLevel()
    {
        // PlayerSkills skills = new PlayerSkills();
        //
        // int mag = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Magic).Experience);
        // int ran = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Ranged).Experience);
        // int att = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Attack).Experience);
        // int str = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Strength).Experience);
        // int def = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Defence).Experience);
        // int hp = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Hitpoints).Experience);
        // int pray = Skill.GetLevelForXP(skills.GetSkill(SkillEnum.Prayer).Experience);
        //
        // if (ran > att + str)
        // {
        //     Data.CombatLevel = (int)Math.Floor((def * 0.25) + (hp * 0.25) + (pray * 0.125) + (ran * 0.4875));
        // }
        // else if (mag > att + str)
        // {
        //     Data.CombatLevel = (int)Math.Floor((def * 0.25) + (hp * 0.25) + (pray * 0.125) + (mag * 0.4875));
        // }
        // else
        // {
        //     Data.CombatLevel =
        //         (int)Math.Floor((def * 0.25) + (hp * 0.25) + (pray * 0.125) + (att * 0.325) + (str * 0.325));
        // }
    }
    
    public void Reset()
    {
        Combat.PerformedHit = false;
        Combat.WasHit = false;
        
        IsUpdateRequired = false;
        IsAppearanceUpdate = false;
        PlacementOrTeleport = false;

        MovementHandler.PrimaryDirection = -1;
        MovementHandler.SecondaryDirection = -1;
        
        Flags = PlayerUpdateFlags.None;
    }

    public void Respawn()
    {
        // Combat.Reset();
        CurrentHealth = 10;
        PerformAnimation(-1);
        IsUpdateRequired = true;
        Flags |= PlayerUpdateFlags.Appearance;
    }
}