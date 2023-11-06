using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Outgoing;

public class SetSkillLevelPacket
{
    private readonly Player _player;

    public SetSkillLevelPacket(Player player)
    {
        _player = player;
    }

    /// <summary>
    /// Sets a player's skill level and experience.
    /// Note that the actual skill level is dictated
    /// by the provided experience based on the game's experience-to-level calculation algorithm.
    /// If, for instance, the experience is 300 but the level parameter is provided as 25, 
    /// the actual level displayed on the client will be 25/4 because the experience dictates the true level.
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="experience"></param>
    /// <param name="level"></param>
    public void Add(SkillEnum skill, int experience, int level)
    {
        _player.Writer.CreateFrame(ServerOpCodes.PLAYER_SKILL);
        _player.Writer.WriteByte((int)skill);
        _player.Writer.WriteDWordV1(experience);
        _player.Writer.WriteByte(level);
    }
}