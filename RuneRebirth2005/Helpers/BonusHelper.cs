namespace RuneRebirth2005.Helpers;

public class BonusHelper
{
    public static List<Bonus> BonusMap { get; set; } = BonusMap = new List<Bonus>
    {
        new Bonus("Stab", 0, 1675),
        new Bonus("Slash", 1, 1676),
        new Bonus("Crush", 2, 1677),
        new Bonus("Magic", 3, 1678),
        new Bonus("Range", 4, 1679),
        /* Defensive */
        new Bonus("Stab", 5, 1680),
        new Bonus("Slash", 6, 1681),
        new Bonus("Crush", 7, 1682),
        new Bonus("Magic", 8, 1683),
        new Bonus("Range", 9, 1684),
        /* Other */
        new Bonus("Strength", 10, 1686),
        new Bonus("Prayer", 11, 1687)
    };
}

public class Bonus
{
    public string Name { get; private set; }
    public int Index { get; private set; }
    public int FrameId { get; private set; }

    public Bonus(string name, int index, int frameId)
    {
        Name = name;
        Index = index;
        FrameId = frameId;
    }
}