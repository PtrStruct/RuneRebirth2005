using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Outgoing;

public class SetPlayerOptionsPacket
{
    private readonly Player _player;

    public SetPlayerOptionsPacket(Player player)
    {
        _player = player;
    }

    public void Add(int actionIndex, bool actionOnTop, string actionText)
    {
        _player.Writer.CreateFrameVarSize(ServerOpCodes.PLAYER_RIGHTCLICK);
        _player.Writer.WriteByteC(actionIndex);
        _player.Writer.WriteByteA(actionOnTop ? 1 : 0);
        _player.Writer.WriteString(actionText);
        _player.Writer.EndFrameVarSize();
    }
}