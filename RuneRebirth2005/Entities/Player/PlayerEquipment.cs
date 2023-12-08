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

    public bool EquipItem(int itemID, string name, int amount = 1)
    {
        if (GameConstants.IsItemInArray(itemID, GameConstants.BOWS) || GameConstants.IsItemInArray(itemID, GameConstants.OTHER_RANGE_WEAPONS))
        {
            SetItem(EquipmentSlot.Weapon, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.ARROWS))
        {
            SetItem(EquipmentSlot.Ammo, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.capes))
        {
            SetItem(EquipmentSlot.Cape, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.boots))
        {
            SetItem(EquipmentSlot.Boots, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.gloves))
        {
            SetItem(EquipmentSlot.Gloves, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.shields))
        {
            SetItem(EquipmentSlot.Shield, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.hats))
        {
            SetItem(EquipmentSlot.Helmet, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.amulets))
        {
            SetItem(EquipmentSlot.Amulet, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.rings))
        {
            SetItem(EquipmentSlot.Ring, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.body))
        {
            SetItem(EquipmentSlot.Chest, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.legs))
        {
            SetItem(EquipmentSlot.Legs, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
        }
        else
        {
            SetItem(EquipmentSlot.Weapon, new EquipmentItem { ItemId = itemID, Quantity = amount, Name = name });
            // Log.Warning($"Matching item type not found for item id: {itemID}.");
            // return false;
        }

        return true;
    }

    public void SetItem(EquipmentSlot slot, EquipmentItem item)
    {
        if (EquipmentSlots.ContainsKey(slot))
        {
            EquipmentSlots[slot] = item;
        }
        else
        {
            Log.Warning($"Slot does not exist: {slot}.");
        }
    }
    
    public EquipmentSlot GetEquipmentSlotByItemId(int itemID)
    {
        if (GameConstants.IsItemInArray(itemID, GameConstants.BOWS) || GameConstants.IsItemInArray(itemID, GameConstants.OTHER_RANGE_WEAPONS))
        {
            return EquipmentSlot.Weapon;
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.ARROWS))
        {
            return EquipmentSlot.Ammo;
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.capes))
        {
            return EquipmentSlot.Cape;
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.boots))
        {
            return EquipmentSlot.Boots;
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.gloves))
        {
            return EquipmentSlot.Gloves;
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.shields))
        {
            return EquipmentSlot.Shield;
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.hats))
        {
            return EquipmentSlot.Helmet;
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.amulets))
        {
            return EquipmentSlot.Amulet;
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.rings))
        {
            return EquipmentSlot.Ring;
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.body))
        {
            return EquipmentSlot.Chest;
        }
        else if (GameConstants.IsItemInArray(itemID, GameConstants.legs))
        {
            return EquipmentSlot.Legs;
        }
        else
        {
            // Default to Weapon if no match
            return EquipmentSlot.Weapon;
        }
    }
    
}

public class EquipmentItem
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; }
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