using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Outgoing;

public class SetSidebarInterfacePacket
{
    private readonly Player _player;

    public SetSidebarInterfacePacket(Player player)
    {
        _player = player;
    }

    public void Add(int _form, int _menuId)
    {
        _player.Writer.CreateFrame(ServerOpCodes.SIDEBAR_INTF_ASSIGN);
        _player.Writer.WriteWord(_menuId); /* Tab */
        _player.Writer.WriteByteA(_form); /* Icon */
    }
}