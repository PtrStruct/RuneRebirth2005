using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;

namespace RuneRebirth2005.Entities;

public struct PlayerBonuses
{
    public int[] Bonuses { get; set; } = new int[13];

    public PlayerBonuses()
    {
    }

    public void SetBonus(int index, int value)
    {
        Bonuses[index] = value;
    }

    public void AddBonus(int index, int amount)
    {
        Bonuses[index] += amount;
    }

    public int GetBonus(int index)
    {
        return Bonuses[index];
    }
}