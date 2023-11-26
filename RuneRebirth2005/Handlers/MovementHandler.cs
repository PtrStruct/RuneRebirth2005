using Microsoft.CSharp.RuntimeBinder;
using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;
using RuneRebirth2005.World;
using RuneRebirth2005.World.Clipping;
using Serilog;

namespace RuneRebirth2005.Handlers;

public class MovementHandler
{
    public int PrimaryDirection { get; set; } = -1;
    public int SecondaryDirection { get; set; } = -1;
    public bool DiscardMovementQueue { get; set; }

    private readonly Character _character;
    public bool IsRunToggled { get; set; }

    public Character FollowCharacter { get; set; }

    private readonly LinkedList<Point> waypoints = new();

    public MovementHandler(Character character)
    {
        _character = character;
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
        if (_character.CurrentHealth <= 0)
        {
            return;
        }
        
        /* Handle Follow */
        Follow();

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

        if (_character.Location.IsOutside && _character is Player player)
        {
            SendNewBuildArea();
            player.PacketSender.SendMessage("Sent new build area.");
        }
    }

    private void Follow()
    {
        if (_character != null && FollowCharacter != null)
        {
            Reset();

            var distance = _character.Location.GetDistance(FollowCharacter.Location);
            if (distance <= FollowCharacter.Size)
            {
                return;
            }

            if (_character is Player player)
            {
                player.PacketSender.SendMessage($"Following NPC Size: {FollowCharacter.Size}");
            }

            var destination = PathGenerator.GetCombatPath(_character, FollowCharacter);

            var tiles = new List<Location>();
            tiles = PathFinder.getPathFinder().FindPath(_character, destination.X, destination.Y, true, 16, 16);
            if (tiles != null)
            {
                for (var i = 0; i < tiles.Count; i++) AddToPath(tiles[i]);
                /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
                Finish();
            }
        }
    }

    private Point GetRunPoint()
    {
        if (waypoints.Count > 1 && IsRunToggled)
        {
            var runPoint = waypoints.First.Value;
            waypoints.RemoveFirst();
            return runPoint;
        }

        return null;
    }

    private void MoveToDirection(int direction)
    {
        Log.Information($"Before Move CharacterX: {_character.Location.X} - CharacterY: {_character.Location.Y}");
        _character.Location.Move(MovementHelper.DIRECTION_DELTA_X[direction],
            MovementHelper.DIRECTION_DELTA_Y[direction]);
        Log.Information($"After Move CharacterX: {_character.Location.X} - CharacterY: {_character.Location.Y}");
    }

    private void SendNewBuildArea()
    {
        if (_character is Player player)
        {
            player.PacketSender.BuildNewBuildAreaPacket();
        }
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
        var location = _character.Location;
        waypoints.AddLast(new Point(location.X, location.Y, -1));
    }

    public void Finish()
    {
        waypoints.RemoveFirst();
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