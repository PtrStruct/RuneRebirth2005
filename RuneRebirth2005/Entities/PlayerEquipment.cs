using System.Reflection;

namespace RuneRebirth2005.Entities;

public class PlayerEquipment
{
    public int Helmet { get; set; } = -1;
    public int Amulet { get; set; } = -1;
    public int Cape { get; set; } = -1;
    public int Weapon { get; set; } = -1;
    public int Body { get; set; } = -1;
    public int Shield { get; set; } = -1;
    public int Gloves { get; set; } = -1;
    public int Legs { get; set; } = -1;
    public int Boots { get; set; } = -1;

    public IEnumerable<int> GetEquipment()
    {
        return new List<int>
        {
            Helmet,
            Amulet,
            Cape,
            Weapon,
            Body,
            Shield,
            Gloves,
            Legs,
            Boots
        };
    }
}