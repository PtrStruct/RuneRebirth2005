using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Outgoing;

public class SendPlayerMessage
{
    private readonly Player _player;
    private readonly int _x;
    private readonly int _y;

    public SendPlayerMessage(Player player)
    {
        _player = player;
        _x = player.Location.CenterChunkX;
        _y = player.Location.CenterChunkY;
    }

    public void Add(string msg)
    {
        _player.Writer.CreateFrameVarSize(ServerOpCodes.MSG_SEND);
        _player.Writer.WriteString(msg);
        _player.Writer.EndFrameVarSize();
    }
}