using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Outgoing;

public class SetSidebarInterfacePacket
{
    private readonly Player _player;

    public SetSidebarInterfacePacket(Player player)
    {
        _player = player;
    }

    public void Add(int _tabId, int _displayId)
    {
        _player.Writer.CreateFrame(ServerOpCodes.SIDEBAR_INTF_ASSIGN);
        _player.Writer.WriteWord(_displayId); /* What to display inside that tab */
        _player.Writer.WriteByteA(_tabId); /* Which tab */
    }
}