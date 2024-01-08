using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;
using RuneRebirth2005.World;
using RuneRebirth2005.World.Clipping;
using Serilog;

namespace RuneRebirth2005.Handlers;

public class MovementHandler
{
    private int[] newWalkCmdX = new int[walkingQueueSize];
    private int[] newWalkCmdY = new int[walkingQueueSize];
    public int newWalkCmdSteps = 0;
    private bool newWalkCmdIsRunning = false;
    protected int[] travelBackX = new int[walkingQueueSize];
    protected int[] travelBackY = new int[walkingQueueSize];
    protected int numTravelBackSteps = 0;
    public const int walkingQueueSize = 50;
    public int[] walkingQueueX = new int[walkingQueueSize];
    public int[] walkingQueueY = new int[walkingQueueSize];
    public int wQueueReadPtr = 0;
    public int wQueueWritePtr = 0;

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

            if (_character is Player player)
            {
                // player.PacketSender.SendMessage($"Following NPC Size: {FollowCharacter.Size}");

                var destination = PathGenerator.GetCombatPath(_character, FollowCharacter);
                var tiles = new List<Location>();

                player.PacketSender.SendMessage(
                    $"Following NPC Size: {FollowCharacter.Size} to Position: X: {destination.X} - Y: {destination.Y}");

                tiles = PathFinder.getPathFinder().FindPath(_character, destination.X, destination.Y, true, 16, 16);

                if (tiles != null)
                {
                    for (var i = 0; i < tiles.Count; i++) AddToPath(tiles[i]);
                    /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
                    Finish();
                }
            }
            else if (_character is NPC theNpc)
            {
                var npcX = theNpc.Location.X;
                var npcY = theNpc.Location.Y;

                if (theNpc.Location.Equals(FollowCharacter.Location))
                {
                    Reset();
                    /* Too predictable? */
                    StepAway(theNpc);
                    Finish();
                    return;
                }

                var nextTile = theNpc.DumbPathFinder.Follow(_character, FollowCharacter);
                if (nextTile != null)
                {
                    AddToPath(new Location(npcX + nextTile.X, npcY + nextTile.Y));
                    Finish();
                }
            }
        }
    }

    private void StepAway(Character character)
    {
        if (character.MovementHandler.CanWalk(-1, 0))
            character.MovementHandler.AddToPath(new Location(character.Location.X - 1, character.Location.Y + 0));
        else if (character.MovementHandler.CanWalk(1, 0))
            character.MovementHandler.AddToPath(new Location(character.Location.X + 1, character.Location.Y + 0));
        else if (character.MovementHandler.CanWalk(0, character.Location.Y + -1))
            character.MovementHandler.AddToPath(new Location(character.Location.X, character.Location.Y + -1));
        else if (character.MovementHandler.CanWalk(0, 1))
            character.MovementHandler.AddToPath(new Location(character.Location.X, character.Location.Y + 1));
    }

    public void NPCRandomWalk(Character character)
    {
        var npc = character as NPC;
        if (npc.Stationary)
            return;

        var spawnX = npc.SpawnLocation.X;
        var spawnY = npc.SpawnLocation.Y;
        
        Random random = new Random();
        int direction = random.Next(0, 10); // Generates a random number between 0 and 3 (inclusive)

        int deltaX = 0;
        int deltaY = 0;

        switch (direction)
        {
            case 0:
                deltaX = -1;
                break;
            case 1:
                deltaX = 1;
                break;
            case 2:
                deltaY = -1;
                break;
            case 3:
                deltaY = 1;
                break;
        }

        if (direction <= 3 && IsWithinRange(npc.Location.X, npc.Location.Y, spawnX, spawnY, 5) && character.MovementHandler.CanWalk(deltaX, deltaY))
        {
            character.MovementHandler.AddToPath(new Location(character.Location.X + deltaX, character.Location.Y + deltaY));
        }
    }

    
    public static bool IsWithinRange(int LocationX, int LocationY, int SpawnX, int SpawnY, double range)
    {
        // Calculate the distance between LocationX, LocationY, and SpawnX, SpawnY
        double distance = Math.Sqrt(Math.Pow(LocationX - SpawnX, 2) + Math.Pow(LocationY - SpawnY, 2));

        // Check if the calculated distance is less than or equal to the specified range
        return distance <= range;
    }
    
    bool CanWalk(int deltaX, int deltaY)
    {
        Location to = new Location(_character.Location.X + deltaX, _character.Location.Y + deltaY);

        return canWalk(_character.Location, to, _character.Size);
    }

    static bool canWalk(Location from, Location to, int size)
    {
        return Region.canMove(from, to, size, size);
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
        //Log.Information($"Before Move CharacterX: {_character.Location.X} - CharacterY: {_character.Location.Y}");
        _character.Location.Move(MovementHelper.DIRECTION_DELTA_X[direction],
            MovementHelper.DIRECTION_DELTA_Y[direction]);
        //Log.Information($"After Move CharacterX: {_character.Location.X} - CharacterY: {_character.Location.Y}");
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