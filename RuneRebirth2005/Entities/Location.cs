using Serilog;

namespace RuneRebirth2005.Entities;

public class Location
{
    private int _x, _y, _z;

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

    public int Z
    {
        get => _z;
        set { _z = value; }
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

    public Player Player { get; set; }

    public Location(int x, int y)
    {
        X = x;
        Y = y;
        ForceUpdate();
        // Log.Warning($"RegionId {RegionId} - ChunkX: {CenterChunkX} - ChunkY: {CenterChunkY}");
    }

    private void Update()
    {
        if (IsOutside)
        {
            CenterChunkX = X >> 3;
            CenterChunkY = Y >> 3;
            RegionId = (((X >> 6) << 8) & 0xFF00) | ((Y >> 6) & 0xFF);
            OffsetChunkX = CenterChunkX - 6;
            OffsetChunkY = CenterChunkY - 6;
            BuildAreaStartX = OffsetChunkX << 3;
            BuildAreaStartY = OffsetChunkY << 3;
            
            if (Player != null)
            {
                Player.PacketSender.BuildNewBuildAreaPacket();
            }
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

    internal void Move(int amountX, int amountY)
    {
        X += amountX;
        Y += amountY;
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
    
    public bool IsWithinDistance(Location other, int distance) {
        if(Z != other.Z)
            return false;
        
        int deltaX = Math.Abs(X - other.X);
        int deltaY = Math.Abs(Y - other.Y);
        return deltaX <= distance && deltaY <= distance;
    }
    
    public Location[] GetOuterTiles(int size)
    {
        Location[] tiles = new Location[size * 4];
        int index = 0;

        for (int x = 0; x < size; x++)
        {
            tiles[index++] = new Location(X + x, Y - 1);
            tiles[index++] = new Location(X + x, Y + size);
        }

        for (int y = 0; y < size; y++)
        {
            tiles[index++] = new Location(X - 1, Y + y);
            tiles[index++] = new Location(X + size, Y + y);
        }

        return tiles;
    }
    
    public static bool IsPlayerInsideNPC(IEntity player, IEntity npc)
    {
        var npcBottomLeft = npc.Location;
        var npcSize = npc.Size;
        var npcTopRight = new Location(npcBottomLeft.X + npcSize - 1, npcBottomLeft.Y + npcSize - 1);
    
        var playerLocation = player.Location;
        var playerXInside = (playerLocation.X >= npcBottomLeft.X) & (playerLocation.X <= npcTopRight.X);
        var playerYInside = (playerLocation.Y >= npcBottomLeft.Y) & (playerLocation.Y <= npcTopRight.Y);
    
        return playerXInside & playerYInside;
    }
    
    public int GetDistance(Location other) {
        int deltaX = X - other.X;
        int deltaY = Y - other.Y;
        return (int) Math.Ceiling(Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
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
    
    public override bool Equals(object obj) 
    {
        var loc = obj as Location;
        if(loc == null) 
            return false; 
        return X == loc.X && Y == loc.Y && Z == loc.Z;
    }

    public override int GetHashCode() 
    {
        return HashCode.Combine(X, Y, Z);
    }
    
}