namespace RuneRebirth2005.Fighting;

public class Projectile
{
    public record AmmunitionData(int ItemId, Graphic ProjectileGraphic, int ProjectileId, int Strength);
    public record Graphic(int ProjectileId, GraphicHeight Height);
    
    public static AmmunitionData BRONZE_ARROW = new AmmunitionData(882, new Graphic(19, GraphicHeight.HIGH), 10, 7);
    public static AmmunitionData IRON_ARROW = new AmmunitionData(884, new Graphic(18, GraphicHeight.HIGH), 9, 10);
    public static AmmunitionData STEEL_ARROW = new AmmunitionData(886, new Graphic(20, GraphicHeight.HIGH), 11, 16);
    public static AmmunitionData MITHRIL_ARROW = new AmmunitionData(888, new Graphic(21, GraphicHeight.HIGH), 12, 22);
    public static AmmunitionData ADAMANT_ARROW = new AmmunitionData(890, new Graphic(22, GraphicHeight.HIGH), 13, 31);
    public static AmmunitionData RUNE_ARROW = new AmmunitionData(892, new Graphic(24, GraphicHeight.HIGH), 15, 50);
    
    public enum GraphicHeight
    {
        HIGH,
        MEDIUM,
        LOW
    }
}