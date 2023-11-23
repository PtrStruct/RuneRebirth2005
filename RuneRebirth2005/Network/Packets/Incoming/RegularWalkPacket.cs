using RuneRebirth2005.Entities;
using RuneRebirth2005.World.Clipping;

namespace RuneRebirth2005.Network.Incoming;

public class RegularWalkPacket : IPacket
{
    private PacketParameters _packetParameters;
    private string _username;
    private Player _player;
    private int _firstStepX;
    private int _firstStepY;
    private bool _running;
    private int _destX;
    private int _destY;
    private int _length;
    private int _steps;
    private int[,] _path;

    public RegularWalkPacket(PacketParameters packetParameters)
    {
        _packetParameters = packetParameters;
        _player = _packetParameters.Player;

        _destX = -1;
        _destY = -1;
        _length = packetParameters.Length;

        _steps = (_length - 5) / 2;
        _path = new int[_steps, 2];

        _firstStepX = _player.PlayerSession.Reader.ReadSignedWordBigEndianA();
        for (var i = 0; i < _steps; i++)
        {
            _path[i, 0] = (sbyte)_player.PlayerSession.Reader.ReadUnsignedByte();
            _path[i, 1] = (sbyte)_player.PlayerSession.Reader.ReadUnsignedByte();
        }
        _firstStepY = _player.PlayerSession.Reader.ReadSignedWordBigEndian();
        _running = _player.PlayerSession.Reader.ReadSignedByteC() == 1;
    }


    public void Process()
    {
        _player.IsUpdateRequired = true;
        
        _player.Combat.Target = null;
        // _player.NPCCombatFocus = null;
        _player.MovementHandler.Reset();
        _player.MovementHandler.SetRunToggled(_running);

        // Console.WriteLine($"X: {firstStepX} - Y: {firstStepY} - Running: {running}");
        var x = _firstStepX;
        var y = _firstStepY;
        for (var i = 0; i < _steps; i++)
        {
            _path[i, 0] += _firstStepX;
            _path[i, 1] += _firstStepY;
            _destX = _path[i, 0];
            _destY = _path[i, 1];
            /* Add x, y to path */
        }

        //
        // /* Used in order to interrupt any ongoing tasks */
        //
        
        var tiles = new List<Location>();
        if (_path.Length > 0)
            tiles = PathFinder.getPathFinder().FindRoute(_player, _destX, _destY, true, 1, 1);
        else
            tiles = PathFinder.getPathFinder().FindRoute(_player, x, y, true, 1, 1);

        if (tiles != null)
        {
            for (var i = 0; i < tiles.Count; i++) _player.MovementHandler.AddToPath(tiles[i]);

            /* Remove the first waypoint, aka the tile we're standing on, otherwise it'll take an extra tick to start walking */
            _player.MovementHandler.Finish();
        }
        //
        // Console.WriteLine($"Built {nameof(RegularWalkPacket)}");
    }
}