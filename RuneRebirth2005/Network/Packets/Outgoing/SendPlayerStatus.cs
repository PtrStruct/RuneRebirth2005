using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Outgoing;


public class SendPlayerStatus
{
    private readonly Player _player;
    private readonly int _x;
    private readonly int _y;

    public SendPlayerStatus(Player player)
    {
        _player = player;
        _x = player.Location.CenterChunkX;
        _y = player.Location.CenterChunkY;
    }

    public void Add()
    {
        _player.Writer.CreateFrame(ServerOpCodes.PLAYER_STATUS);
        _player.Writer.WriteByteA(0);
        _player.Writer.WriteWordBigEndianA(_player.Index);
    }
}