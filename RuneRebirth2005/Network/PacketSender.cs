using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network;

public class PacketSender
{
    private readonly Player _player;

    public PacketSender(Player player)
    {
        _player = player;
    }

    public void LoadRegionPacket()
    {
        _player.PlayerSession.Writer.CreateFrame(ServerOpCodes.REGION_LOAD);
        _player.PlayerSession.Writer.WriteWordA(_player.Location.CenterChunkX);
        _player.PlayerSession.Writer.WriteWord(_player.Location.CenterChunkY);
    }
    
    public void SendPlayerStatus()
    {
        _player.PlayerSession.Writer.CreateFrame(ServerOpCodes.PLAYER_STATUS);
        _player.PlayerSession.Writer.WriteByteA(0);
        _player.PlayerSession.Writer.WriteWordBigEndianA(_player.Index);
    }

    public void SendMessage(string message)
    {
        _player.PlayerSession.Writer.CreateFrameVarSize(ServerOpCodes.MSG_SEND);
        _player.PlayerSession.Writer.WriteString(message);
        _player.PlayerSession.Writer.EndFrameVarSize();
    }

    public void SendTextToInterface(string _text, int _interfaceId)
    {
        _player.PlayerSession.Writer.CreateFrameVarSizeWord(ServerOpCodes.INTF_TEXT_ADD);
        _player.PlayerSession.Writer.WriteString(_text);
        _player.PlayerSession.Writer.WriteWordA(_interfaceId);
        _player.PlayerSession.Writer.EndFrameVarSizeWord();
    }

    public void UpdateSlot(EquipmentSlot slot, int itemId, int amount)
    {
        _player.PlayerSession.Writer.CreateFrameVarSizeWord(ServerOpCodes.ITEM_SLOT_SET);
        _player.PlayerSession.Writer.WriteWord(1688);
        _player.PlayerSession.Writer.WriteByte((int)slot);
        _player.PlayerSession.Writer.WriteWord(itemId + 1);
        if (amount > 254)
        {
            _player.PlayerSession.Writer.WriteByte(255);
            _player.PlayerSession.Writer.WriteDWord(amount);
        }
        else
        {
            _player.PlayerSession.Writer.WriteByte(amount);
        }
        _player.PlayerSession.Writer.EndFrameVarSizeWord();
    }

    public void SendSidebarInterface(int _tabId, int _displayId)
    {
        _player.PlayerSession.Writer.CreateFrame(ServerOpCodes.SIDEBAR_INTF_ASSIGN);
        _player.PlayerSession.Writer.WriteWord(_displayId); /* What to display inside that tab */
        _player.PlayerSession.Writer.WriteByteA(_tabId); /* Which tab */
    }
}