using RuneRebirth2005.Helpers;

namespace RuneRebirth2005.Entities;

public class BonusManager
{
    private const int TotalBonuses = 13;
    private const int NoItemId = -1;
    private static List<EquipmentSlot> equipmentSlots;

    static BonusManager()
    {
        equipmentSlots = Enum.GetValues(typeof(EquipmentSlot)).Cast<EquipmentSlot>().ToList();
    }

    public static void RefreshBonus(Player player)
    {
        ClearPlayerBonuses(player);

        var bonusDefinitions = BonusDefinitionLoader.ItemDefinitions;

        foreach (var equipmentSlot in equipmentSlots)
            UpdateEquipmentBonus(equipmentSlot, player, bonusDefinitions);
    }

    private static void ClearPlayerBonuses(Player player)
    {
        for (int j = 0; j < TotalBonuses; j++)
            player.Bonuses.SetBonus(j, 0);
    }

    private static void UpdateEquipmentBonus(EquipmentSlot equipment, Player player, Root bonusDefinitions)
    {
        var item = player.Equipment.GetItem(equipment);
        if (item.ItemId == NoItemId)
        {
           
            return;
        }

        var itemBonuses = bonusDefinitions.List.ItemBonusDefinition[item.ItemId].Bonuses.Short;

        UpdateBonusWithEquipment(itemBonuses, player);
        UpdateInterfaceAndNotifyPlayer(equipment, item, player);
    }

    private static void UpdateBonusWithEquipment(List<int> itemBonuses, Player player)
    {
        for (int j = 0; j < itemBonuses.Count; j++)
            player.Bonuses.AddBonus(j, itemBonuses[j]);
    }

    private static void UpdateInterfaceAndNotifyPlayer(EquipmentSlot equipment, EquipmentItem item, Player player)
    {
        for (int i = 0; i < BonusHelper.BonusMap.Count; i++)
        {
            var bonus = BonusHelper.BonusMap[i];

            int bonusValue = player.Bonuses.GetBonus(bonus.Index);
            string sign = bonusValue > 0 ? "+" : "";
            // new TextToInterfacePacket(player).Add($"{bonus.Name}: {sign}{bonusValue}", BonusHelper.BonusMap[i].FrameId);
        }

        // new UpdateSlotPacket(player).Add(equipment, item.ItemId, item.Quantity);
    }
}