using System.Reflection;
using Serilog;

namespace RuneRebirth2005.Entities;

public class PlayerEquipment
{
    public Dictionary<EquipmentSlot, EquipmentItem> EquipmentSlots { get; set; }

    public PlayerEquipment()
    {
        EquipmentSlots = new Dictionary<EquipmentSlot, EquipmentItem>()
        {
            { EquipmentSlot.Helmet, new EquipmentItem { ItemId = -1, Quantity = 0 } },
            { EquipmentSlot.Cape, new EquipmentItem { ItemId = -1, Quantity = 0 } },
            { EquipmentSlot.Amulet, new EquipmentItem { ItemId = -1, Quantity = 0 } },
            { EquipmentSlot.Weapon, new EquipmentItem { ItemId = -1, Quantity = 0 } },
            { EquipmentSlot.Chest, new EquipmentItem { ItemId = -1, Quantity = 0 } },
            { EquipmentSlot.Shield, new EquipmentItem { ItemId = -1, Quantity = 0 } },
            { EquipmentSlot.Legs, new EquipmentItem { ItemId = -1, Quantity = 0 } },
            { EquipmentSlot.Gloves, new EquipmentItem { ItemId = -1, Quantity = 0 } },
            { EquipmentSlot.Boots, new EquipmentItem { ItemId = -1, Quantity = 0 } },
            { EquipmentSlot.Ring, new EquipmentItem { ItemId = -1, Quantity = 0 } },
            { EquipmentSlot.Ammo, new EquipmentItem { ItemId = -1, Quantity = 0 } }
        };
    }

    public EquipmentItem GetItem(EquipmentSlot slot)
    {
        return EquipmentSlots.TryGetValue(slot, out var item) ? item : new EquipmentItem { ItemId = -1, Quantity = 0 };
    }

    public void SetItem(EquipmentSlot slot, EquipmentItem item)
    {
        if (EquipmentSlots.ContainsKey(slot))
        {
            EquipmentSlots[slot] = item;
        }
    }
}

public class EquipmentItem
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}

public enum EquipmentSlot
{
    Helmet = 0,
    Cape = 1,
    Amulet = 2,
    Weapon = 3,
    Chest = 4,
    Shield = 5,
    Legs = 7,
    Gloves = 9,
    Boots = 10,
    Ring = 12,
    Ammo = 13
}