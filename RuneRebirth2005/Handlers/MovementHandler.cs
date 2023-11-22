using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;
using RuneRebirth2005.World.Clipping;

namespace RuneRebirth2005.Handlers;

public class MovementHandler
{
    public int PrimaryDirection { get; set; } = -1;
    public int SecondaryDirection { get; set; } = -1;
    public bool DiscardMovementQueue { get; set; }

    private readonly Player _client;
    private bool _runToggled;
    private readonly LinkedList<Point> waypoints = new();

    public MovementHandler(Player client)
    {
        _client = client;
    }
    
    // private bool IsWithinRange()
    // {
    //     var targetLocation = _client.CombatTarget.Location;
    //     var entityLocation = _client.Location;
    //     var entitySize = _client.Size;
    //     var targetSize = _client.CombatTarget.Size;
    //
    //     var horizontallyInRange = IsInRange(entityLocation.X, targetLocation.X, entitySize, targetSize);
    //     var verticallyInRange = IsInRange(entityLocation.Y, targetLocation.Y, entitySize, targetSize);
    //     return horizontallyInRange && verticallyInRange;
    // }

    private bool IsInRange(int entityCoordinate, int targetCoordinate, int entitySize, int targetSize) =>
        entityCoordinate >= targetCoordinate - entitySize && entityCoordinate <= targetCoordinate + targetSize;

    public void Process()
    {
        /* Combat Follow */
        // if (_client.CombatTarget != null && !IsWithinRange())
        // {
        //     if (_client.CombatHandler.CombatMethod is RangeCombat)
        //     {
        //         var targetLocation = _client.CombatTarget.Location;
        //         var entityLocation = _client.Location;
        //         var entitySize = _client.Size;
        //         var targetSize = _client.CombatTarget.Size;
        //
        //         if (!PathFinder.isProjectilePathClear(entityLocation.X, entityLocation.Y, 0, targetLocation.X,
        //                 targetLocation.Y) || !IsWithinRange())
        //         {
        //             PacketBuilder.SendMessage("Range Following..", _client);
        //             _client.MovementHandler.Reset();
        //
        //             var x = _client.CombatTarget.Location.X;
        //             var y = _client.CombatTarget.Location.Y;
        //
        //             /* Follow */
        //             var tiles = new List<Location>();
        //             tiles = PathFinder.getPathFinder().FindRoute(_client, x, y, true, _client.CombatTarget.Size,
        //                 _client.CombatTarget.Size);
        //
        //             if (tiles != null)
        //             {
        //                 for (var i = 0; i < tiles.Count; i++) _client.MovementHandler.AddToPath(tiles[i]);
        //
        //                 /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
        //                 _client.MovementHandler.Finish();
        //             }
        //         }
        //     }
        //     else /* Melee follow */
        //     {
        //         PacketBuilder.SendMessage("Following..", _client);
        //         _client.MovementHandler.Reset();
        //
        //         var x = _client.CombatTarget.Location.X;
        //         var y = _client.CombatTarget.Location.Y;
        //
        //         /* Follow */
        //         var tiles = new List<Location>();
        //         tiles = PathFinder.getPathFinder().FindRoute(_client, x, y, true, _client.CombatTarget.Size,
        //             _client.CombatTarget.Size);
        //
        //         if (tiles != null)
        //         {
        //             for (var i = 0; i < tiles.Count; i++) _client.MovementHandler.AddToPath(tiles[i]);
        //
        //             /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
        //             _client.MovementHandler.Finish();
        //         }
        //     }
        // }

        if (waypoints.Count == 0)
            return;

        var walkPoint = waypoints.First.Value;
        waypoints.RemoveFirst();

        var runPoint = GetRunPoint();

        if (walkPoint != null && walkPoint.Direction != -1)
        {
            MoveToDirection(walkPoint.Direction);
            PrimaryDirection = walkPoint.Direction;
        }

        if (runPoint != null && runPoint.Direction != -1)
        {
            MoveToDirection(runPoint.Direction);
            SecondaryDirection = runPoint.Direction;
        }

        if (_client.Location.IsOutside)
        {
            SendMapRegionPacket();
            Console.WriteLine("Sent Region Packet!");
        }
    }

    private Point GetRunPoint()
    {
        if (waypoints.Count > 1 && GetRunToggled())
        {
            var runPoint = waypoints.First.Value;
            waypoints.RemoveFirst();
            return runPoint;
        }

        return null;
    }

    private void MoveToDirection(int direction)
    {
        _client.Location.Move(MovementHelper.DIRECTION_DELTA_X[direction],
            MovementHelper.DIRECTION_DELTA_Y[direction]);
    }

    private bool IsOutsideMapRegion()
    {
        var deltaX = _client.Location.PositionRelativeToOffsetChunkX;
        var deltaY = _client.Location.PositionRelativeToOffsetChunkY;

        return deltaX < 16 || deltaX >= 88 || deltaY < 16 || deltaY > 88;
    }

    private void SendMapRegionPacket()
    {
        _client.PacketSender.LoadRegionPacket();
    }

    public void AddToPath(Location location)
    {
        if (waypoints.Count == 0)
        {
            Reset();
        }

        var last = waypoints.Last.Value;
        var deltaX = location.X - last.X;
        var deltaY = location.Y - last.Y;
        var max = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));

        if (max > 40)
        {
            Console.WriteLine("ehm, stop that :^)");
            return;
        }

        for (var i = 0; i < max; i++)
        {
            if (deltaX < 0)
            {
                deltaX++;
            }
            else if (deltaX > 0)
            {
                deltaX--;
            }

            if (deltaY < 0)
            {
                deltaY++;
            }
            else if (deltaY > 0)
            {
                deltaY--;
            }

            AddStep(location.X - deltaX, location.Y - deltaY);
        }
    }

    private void AddStep(int x, int y)
    {
        if (waypoints.Count >= 100)
        {
            return;
        }

        var last = waypoints.Last.Value;
        var deltaX = x - last.X;
        var deltaY = y - last.Y;
        var direction = MovementHelper.GetDirection(deltaX, deltaY);

        if (direction > -1)
        {
            waypoints.AddLast(new Point(x, y, direction));
        }
    }

    public void Reset()
    {
        waypoints.Clear();
        var location = _client.Location;
        waypoints.AddLast(new Point(location.X, location.Y, -1));
    }

    public void Finish()
    {
        waypoints.RemoveFirst();
    }

    public void SetRunToggled(bool runPath)
    {
        _runToggled = runPath;
    }

    public bool GetRunToggled()
    {
        return _runToggled;
    }
}

public class Point
{
    public int X { get; }
    public int Y { get; }
    public int Direction { get; }

    public Point(int x, int y, int direction)
    {
        X = x;
        Y = y;
        Direction = direction;
    }
}

public class Tile
{
    public int X { get; }
    public int Y { get; }

    public Tile(int x, int y)
    {
        X = x;
        Y = y;
    }
}