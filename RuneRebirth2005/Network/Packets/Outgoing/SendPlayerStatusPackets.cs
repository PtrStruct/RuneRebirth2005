using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Outgoing;


public class SendPlayerStatusPackets
{
    private readonly Player _player;
    private readonly int _x;
    private readonly int _y;

    public SendPlayerStatusPackets(Player player)
    {
        _player = player;
    }

    public void Add()
    {
        _player.Writer.CreateFrame(ServerOpCodes.PLAYER_STATUS);
        _player.Writer.WriteByteA(0);
        _player.Writer.WriteWordBigEndianA(_player.Index);
    }
}