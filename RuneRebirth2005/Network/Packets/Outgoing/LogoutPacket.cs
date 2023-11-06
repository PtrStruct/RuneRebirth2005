using RuneRebirth2005.Entities;
using Serilog;

namespace RuneRebirth2005.Network.Outgoing;

public class LogoutPacket
{
    private readonly Player _player;

    public LogoutPacket(Player player)
    {
        _player = player;
    }

    public void Add()
    {
        _player.Writer.CreateFrame(ServerOpCodes.DISCONNECT);
        Log.Information($"{_player.Username} has logged out.");
    }
}