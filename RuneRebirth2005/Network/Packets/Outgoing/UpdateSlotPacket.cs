using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Outgoing;

public class UpdateSlotPacket
{
    private readonly Player _player;

    public UpdateSlotPacket(Player player)
    {
        _player = player;
    }

    public void Add(EquipmentSlot slot, int itemId, int amount)
    {
        _player.Writer.CreateFrameVarSizeWord(ServerOpCodes.ITEM_SLOT_SET);
        _player.Writer.WriteWord(1688);
        _player.Writer.WriteByte((int)slot);

        _player.Writer.WriteWord(itemId + 1);
        if (amount > 254)
        {
            _player.Writer.WriteByte(255);
            _player.Writer.WriteDWord(amount);
        }
        else
        {
            _player.Writer.WriteByte(amount);
        }

        _player.Writer.EndFrameVarSizeWord();
    }
}