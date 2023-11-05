using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Outgoing;

public class RegionLoadPacket
{
    private readonly Player _player;
    private readonly int _x;
    private readonly int _y;

    public RegionLoadPacket(Player player)
    {
        _player = player;
        _x = player.Location.CenterChunkX;
        _y = player.Location.CenterChunkY;
    }

    public void Add()
    {
        _player.Writer.CreateFrame(ServerOpCodes.REGION_LOAD);
        _player.Writer.WriteWordA(_x);
        _player.Writer.WriteWord(_y);
    }
}