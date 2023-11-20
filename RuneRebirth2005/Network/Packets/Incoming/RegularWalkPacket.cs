using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Incoming;

public class RegularWalkPacket : IPacket
{
    private PacketParameters _packetParameters;
    private readonly string _username;
    private readonly Player _player;
    private readonly int _firstStepX;
    private readonly int _firstStepY;
    private readonly bool _running;
    private readonly int _destX;
    private readonly int _destY;
    private readonly int _length;
    private readonly int _steps;
    private readonly int[,] _path;

    public RegularWalkPacket(PacketParameters packetParameters)
    {
        // _packetParameters = packetParameters;
        // _player = _packetParameters.Player;
        //
        // _destX = -1;
        // _destY = -1;
        // _length = packetParameters.Length;
        //
        // _steps = (_length - 5) / 2;
        // _path = new int[_steps, 2];
        //
        // _firstStepX = _player.Reader.ReadSignedWordBigEndianA();
        // for (var i = 0; i < _steps; i++)
        // {
        //     _path[i, 0] = (sbyte)_player.Reader.ReadUnsignedByte();
        //     _path[i, 1] = (sbyte)_player.Reader.ReadUnsignedByte();
        // }
        //
        // _firstStepY = _player.Reader.ReadSignedWordBigEndian();
        // _running = _player.Reader.ReadSignedByteC() == 1;
    }


    public void Process()
    {
        // _player.NPCCombatFocus = null;
        // player.MovementHandler.Reset();
        // player.MovementHandler.SetRunToggled(running);
        //
        // Console.WriteLine($"X: {firstStepX} - Y: {firstStepY} - Running: {running}");
        // var x = firstStepX;
        // var y = firstStepY;
        // for (var i = 0; i < steps; i++)
        // {
        //     path[i, 0] += firstStepX;
        //     path[i, 1] += firstStepY;
        //     _destX = path[i, 0];
        //     _destY = path[i, 1];
        //     /* Add x, y to path */
        // }
        //
        // /* Used in order to interrupt any ongoing tasks */
        //
        // //client.IsUpdateRequired = true;
        // var tiles = new List<Location>();
        // if (path.Length > 0)
        //     tiles = PathFinder.getPathFinder().FindRoute(player, _destX, _destY, true, 1, 1);
        // else
        //     tiles = PathFinder.getPathFinder().FindRoute(player, x, y, true, 1, 1);
        //
        // if (tiles != null)
        // {
        //     for (var i = 0; i < tiles.Count; i++) player.MovementHandler.AddToPath(tiles[i]);
        //
        //     /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
        //     player.MovementHandler.Finish();
        // }
        //
        // Console.WriteLine($"Built {nameof(RegularWalkPacket)}");
    }
}