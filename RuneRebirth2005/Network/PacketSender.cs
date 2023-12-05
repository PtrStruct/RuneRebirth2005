using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network;

public class PacketSender
{
    private readonly Player _player;

    public PacketSender(Player player)
    {
        _player = player;
    }

    public void BuildNewBuildAreaPacket()
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
    
    public void CreateProjectile(int x, int y, int offX, int offY, int angle, int speed, int gfxMoving, int startHeight, int endHeight, int lockon, int time)
    {
        if(_player.PlayerSession.Writer != null && _player != null) 
        {
            /* Update Player Location */
            _player.PlayerSession.Writer.CreateFrame(ServerOpCodes.PLAYER_LOCATION);
            _player.PlayerSession.Writer.WriteByteC((y - (_player.Location.OffsetChunkY * 8)) - 2);
            _player.PlayerSession.Writer.WriteByteC((x - (_player.Location.OffsetChunkX * 8)) - 3);
            
            _player.PlayerSession.Writer.CreateFrame(ServerOpCodes.PROJECTILE);
            _player.PlayerSession.Writer.WriteByte(angle);
            _player.PlayerSession.Writer.WriteByte(offY);
            _player.PlayerSession.Writer.WriteByte(offX);
            _player.PlayerSession.Writer.WriteWord(lockon);
            _player.PlayerSession.Writer.WriteWord(gfxMoving);
            _player.PlayerSession.Writer.WriteByte(startHeight);
            _player.PlayerSession.Writer.WriteByte(endHeight);
            _player.PlayerSession.Writer.WriteWord(time);
            _player.PlayerSession.Writer.WriteWord(speed);
            _player.PlayerSession.Writer.WriteByte(16);
            _player.PlayerSession.Writer.WriteByte(64);
        }
    }

    public void UpdateSlot(int slot, int itemId, int amount, int frameId)
    {
        _player.PlayerSession.Writer.CreateFrameVarSizeWord(ServerOpCodes.ITEM_SLOT_SET);
        _player.PlayerSession.Writer.WriteWord(frameId);
        _player.PlayerSession.Writer.WriteByte(slot);
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