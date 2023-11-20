using System.Collections.Frozen;
using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;

namespace RuneRebirth2005;

public class Server
{
    public static Player[] Players { get; set; } = new Player[ServerConfig.MAX_PLAYERS];
    public static FrozenDictionary<int, NPC> NPCs { get; set; }
}