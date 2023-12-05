using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Incoming;

public class EquipItemPacket : IPacket
{
    private readonly Player _player;
    private readonly int _opcode;
    private readonly int _length;
    private readonly int _interfaceId;
    private readonly int _index;
    private readonly int _itemId;

    public EquipItemPacket(PacketParameters parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;

        _itemId = _player.PlayerSession.Reader.ReadSignedWord();
        _index = _player.PlayerSession.Reader.ReadSignedWordA();
        _interfaceId = _player.PlayerSession.Reader.ReadSignedWordA();
    }

    public void Process()
    {
        /* Get Currently Equipped Item */
        var equipmentSlot = _player.Equipment.GetEquipmentSlotByItemId(_itemId);
        var equipptedsItem = _player.Equipment.GetItem(equipmentSlot);
        EquipmentItem temp = new EquipmentItem
        {
            ItemId = -1,
            Quantity = 0
        };
        
        if (equipptedsItem.ItemId != -1)
            temp = equipptedsItem;

        /* Get Item That The Player Clicked On */
        var clickedItem = _player.PlayerInventory.Inventory[_index];
        if (clickedItem.ItemId != _itemId)
        {
            return;
        }

        /* Equip */
        _player.Equipment.EquipItem(clickedItem.ItemId, clickedItem.Amount);

        
        /* Replace */
        _player.PlayerInventory.Inventory[_index] = new InventorySlot
        {
            ItemId = temp.ItemId,
            Amount = temp.Quantity
        };
        
        foreach (var item in _player.Equipment.EquipmentSlots)
        {
            _player.PacketSender.UpdateSlot((int)item.Key, item.Value.ItemId, item.Value.Quantity,
                GameInterfaces.Equipment);
        }

        for (int i = 0; i < _player.PlayerInventory.Inventory.Length; i++)
        {
            var item = _player.PlayerInventory.Inventory[i];
            _player.PacketSender.UpdateSlot(i, item.ItemId, item.Amount, GameInterfaces.DefaultInventory);
        }

        _player.IsAppearanceUpdate = true;
    }
}