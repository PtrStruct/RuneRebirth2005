using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;

namespace RuneRebirth2005;

public class Server
{
    public static Player[] Players { get; } = new Player[ServerConfig.MAX_PLAYERS];

    public static void InitializePlayers()
    {
        for (var i = 0; i < ServerConfig.MAX_PLAYERS; i++)
            Players[i] = new Player();
    }
}