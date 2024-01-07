using CacheReader.World;
using RuneRebirth2005;
using RuneRebirth2005.Data;
using RuneRebirth2005.Data.Items;
using RuneRebirth2005.Data.Npc;
using RuneRebirth2005.Misc;
using RuneRebirth2005.NPCManagement;
using Scape05.Data.ObjectsDef;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.File("Logs/runerebirth2005.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}{SourceContext:l}] {Message}{NewLine}{Exception}",
        theme: new AnsiConsoleTheme(
            new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.LevelInformation] = "\x1b[38;5;33m", // change Information level to blue
                [ConsoleThemeStyle.LevelWarning] = "\x1b[38;5;228m", // change Warning level to yellow
                [ConsoleThemeStyle.LevelError] = "\x1b[38;5;196m", // change Error level to red
                [ConsoleThemeStyle.LevelFatal] = "\x1b[38;5;208m", // change Fatal level to orange
            }))
    .CreateLogger();

void ParseCache(IndexedFileSystem ifs)
{
    new ObjectDefinitionDecoder(ifs).Run();
    new ItemDefinitionDecoder(ifs).Run();
    new NpcDefinitionDecoder(ifs).Run();
}

void LoadRegionFactory(IndexedFileSystem ifs)
{
    RegionFactory.Load(ifs);
    ServerConfig.Startup = false;
}

var ifs = new IndexedFileSystem("../../../Data/cache", true);

try
{
    Benchmarker.MeasureTime(() => ParseCache(ifs), "Parsing cache");
    Benchmarker.MeasureTime(() => LoadRegionFactory(ifs), "Loading regions");

    NPCLoader.Load();

    ServerEngine serverEngine = new ServerEngine();
    serverEngine.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}