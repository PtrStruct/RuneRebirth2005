namespace RuneRebirth2005.Entities;

public class PlayerColors
{
    public int Hair { get; set; } = 5;
    public int Torso { get; set; } = 8;
    public int Legs { get; set; } = 9;
    public int Feet { get; set; } = 5;
    public int Skin { get; set; } = 0;

    public List<int> GetColors()
    {
        return new List<int>
        {
            Hair,
            Torso,
            Legs,
            Feet,
            Skin
        };
    }
}