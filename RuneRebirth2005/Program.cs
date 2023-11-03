using RuneRebirth2005;
using Serilog;
using Serilog.Exceptions;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File("Logs/runerebirth2005.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Server server = new Server();
    Log.Information("Application Starting");
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