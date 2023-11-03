using System.Diagnostics;
using RuneRebirth2005.Network;

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
        Console.WriteLine(
            $"Server can't keep up!\nElapsed: {elapsedMilliseconds} ms\nDeficit: {-sleepTime.TotalMilliseconds} ms");
    }

    private void Tick()
    {
        ConnectionHandler.AcceptClients();
    }
}