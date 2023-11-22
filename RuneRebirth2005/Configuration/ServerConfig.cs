namespace RuneRebirth2005;

public class ServerConfig
{
    public const int TICK_RATE = 600;
    public const int MAX_PLAYERS = 2048;
    public const int BUFFER_SIZE = 4096;
    public const int PORT = 43594;
    public const string SERVER_NAME = "RuneRebirth2005";
    public static readonly string WELCOME_MSG = $"Welcome to {SERVER_NAME}";
    public const int MAX_NPCS = 60000;
    public const int SERVER_EXP_BONUS = 500;
    public static bool Startup { get; set; } = true;
}