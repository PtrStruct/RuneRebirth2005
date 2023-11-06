using RuneRebirth2005.Network.Outgoing;
using Serilog;

namespace RuneRebirth2005.Entities;

public class Location
{
    private int _x, _y;

    public int X
    {
        get => _x;
        set
        {
            _x = value;
            Update();
        }
    }

    public int Y
    {
        get => _y;
        set
        {
            _y = value;
            Update();
        }
    }

    public int CenterChunkX { get; set; }
    public int CenterChunkY { get; set; }
    public int RegionId { get; set; }
    public int OffsetChunkX { get; set; }
    public int OffsetChunkY { get; set; }
    public int BuildAreaStartX { get; set; }
    public int BuildAreaStartY { get; set; }
    public int PositionRelativeToOffsetChunkX => X - OffsetChunkX * 8;
    public int PositionRelativeToOffsetChunkY => Y - OffsetChunkY * 8;

    public bool IsOutside => PositionRelativeToOffsetChunkX < 16 || PositionRelativeToOffsetChunkX >= 88 ||
                             PositionRelativeToOffsetChunkY < 16 || PositionRelativeToOffsetChunkY >= 88;

    public Location(int x, int y)
    {
        X = x;
        Y = y;
        ForceUpdate();
        Log.Warning($"RegionId {RegionId} - ChunkX: {CenterChunkX} - ChunkY: {CenterChunkY}");
    }

    private void Update()
    {
        if (!IsOutside)
        {
            CenterChunkX = X >> 3;
            CenterChunkY = Y >> 3;
            RegionId = (((X >> 6) << 8) & 0xFF00) | ((Y >> 6) & 0xFF);
            OffsetChunkX = CenterChunkX - 6;
            OffsetChunkY = CenterChunkY - 6;
            BuildAreaStartX = OffsetChunkX << 3;
            BuildAreaStartY = OffsetChunkY << 3;
        }
    }

    private void ForceUpdate()
    {
        CenterChunkX = X >> 3;
        CenterChunkY = Y >> 3;
        RegionId = (((X >> 6) << 8) & 0xFF00) | ((Y >> 6) & 0xFF);
        OffsetChunkX = CenterChunkX - 6;
        OffsetChunkY = CenterChunkY - 6;
        BuildAreaStartX = OffsetChunkX << 3;
        BuildAreaStartY = OffsetChunkY << 3;
    }

    public bool IsWithinArea(Location playerLocation)
    {
        var delta = Delta(this, playerLocation);
        return delta.X <= 14 && delta.X >= -15 && delta.Y <= 14 && delta.Y >= -15;
    }

    public static Location Delta(Location a, Location b)
    {
        return new Location(b.X - a.X, b.Y - a.Y);
    }

    public List<string> ToStringParts()
    {
        List<string> messageParts = new List<string>
        {
            $"X: {_x} Y: {_y}",
            $"CenterChunkX: {CenterChunkX} CenterChunkY: {CenterChunkY}",
            $"RegionId: {RegionId} OffsetChunkX: {OffsetChunkX} OffsetChunkY: {OffsetChunkY}",
            $"BuildAreaStartX: {BuildAreaStartX} BuildAreaStartY: {BuildAreaStartY}",
            $"PositionRelativeToOffsetChunkX: {PositionRelativeToOffsetChunkX} PositionRelativeToOffsetChunkY: {PositionRelativeToOffsetChunkY}",
            $"IsOutside: {IsOutside}"
        };

        return messageParts;
    }
}