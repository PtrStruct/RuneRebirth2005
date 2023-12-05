namespace RuneRebirth2005.Entities;

public class PlayerInventory
{
    public InventorySlot[] Inventory { get; set; } = new InventorySlot[28];

    public PlayerInventory()
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            Inventory[i] = new InventorySlot
            {
                ItemId = -1,
                Amount = 0
            };
        }
    }

    public void AddItem(int itemId, int amount = 1)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i].ItemId != -1) continue;

            Inventory[i] = new InventorySlot
            {
                ItemId = itemId,
                Amount = amount
            };

            break;
        }
    }
}

public class InventorySlot
{
    public int ItemId { get; set; }
    public int Amount { get; set; }
}