using System.Diagnostics;
using RuneRebirth2005.Network;
using RuneRebirth2005.Network.Outgoing;
using Serilog;

namespace RuneRebirth2005;

public class ServerEngine
{
    private bool _isRunning;

    public void Run()
    {
        _isRunning = true;

        ConnectionHandler.Initialize();

        while (_isRunning)
        {
            var stopwatch = StartStopwatch();
            Tick();
            SleepIfRequired(stopwatch);
        }
    }

    private Stopwatch StartStopwatch()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        return stopwatch;
    }

    private void SleepIfRequired(Stopwatch stopwatch)
    {
        stopwatch.Stop();
        var sleepTime = CalculateSleepTime(stopwatch.Elapsed.TotalMilliseconds);

        if (sleepTime > TimeSpan.Zero)
            Thread.Sleep(sleepTime);
        else
            WarnAboutDeficit(sleepTime, stopwatch.Elapsed.TotalMilliseconds);
    }

    private TimeSpan CalculateSleepTime(double elapsedMilliseconds)
    {
        return TimeSpan.FromMilliseconds(ServerConfig.TICK_RATE - elapsedMilliseconds);
    }

    private void WarnAboutDeficit(TimeSpan sleepTime, double elapsedMilliseconds)
    {
        Log.Warning(
            $"Server can't keep up!\nElapsed: {elapsedMilliseconds} ms\nDeficit: {-sleepTime.TotalMilliseconds} ms.");
    }

    private void Tick()
    {
        ConnectionHandler.AcceptClients();

        /* Fetch Incoming Data */
        Log.Information("Fetching data from clients..");
        foreach (var player in Server.Players)
        {
            if (player.Index == -1) continue;
            for (int i = 0; i < 50; i++)
                player.PacketHandler.RetrievePacket();
        }

        /* Process Incoming Data */
        Log.Information("Processing fetched data..");
        foreach (var player in Server.Players)
        {
            if (player.Index == -1) continue;
            player.PacketStore.ProcessPackets();
            new PlayerUpdatePacket(player).Add();
        }

        /* Send buffered data */
        Log.Information("Flushing the buffered data!");
        foreach (var player in Server.Players)
        {
            if (player.Index == -1) continue;
            player.FlushBufferedData();
        }
        
        foreach (var player in Server.Players)
        {
            if (player.Index == -1) continue;
            player.Reset();
        }
    }
}