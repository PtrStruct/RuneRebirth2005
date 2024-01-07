using System.Diagnostics;
using RuneRebirth2005.Entities;
using RuneRebirth2005.Network;
using RuneRebirth2005.Update;
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
        DelayedTaskHandler.Tick();

        /* Fetch Incoming Data */
        for (int i = 0; i < Server.Players.Length; i++)
        {
            var player = Server.Players[i];
            if (player == null) continue;
            for (int j = 0; j < 25; j++)
            {
                player.PlayerSession.Fetch(player);
            }
        }
        
        /* Process Incoming Data */
        for (int i = 0; i < Server.Players.Length; i++)
        {
            var player = Server.Players[i];
            if (player == null) continue;
            player.PlayerSession.PacketStore.ProcessPackets();
        }

        /* Combat, calculate damage to perform etc */
        for (int i = 0; i < Server.Players.Length; i++)
        {
            if (Server.Players[i] == null) continue;
            Server.Players[i].Combat.Process();
        }

        for (int i = 0; i < Server.NPCs.Count; i++)
        {
            if (Server.NPCs[i] == null) continue;
            Server.NPCs[i].Combat.Process();
        }

        /* Combat Animations */
        for (int i = 0; i < Server.Players.Length; i++)
        {
            if (Server.Players[i] == null) continue;
            Server.Players[i].Combat.PerformAnimation();
        }

        for (int i = 0; i < Server.NPCs.Count; i++)
        {
            if (Server.NPCs[i] == null) continue;
            Server.NPCs[i].Combat.PerformAnimation();
        }
        
        /* Process new Data for Player and NPC */
        for (int i = 0; i < Server.Players.Length; i++)
        {
            if (Server.Players[i] == null) continue;
            Server.Players[i].Process();
        }

        for (int i = 0; i < Server.NPCs.Count; i++)
        {
            if (Server.NPCs[i] == null) continue;
            Server.NPCs[i].Process();
        }
        
        /* Movement */
        for (int i = 0; i < Server.Players.Length; i++)
        {
            var player = Server.Players[i];
            if (player == null) continue;
            player.MovementHandler.Process();
        }
        
        for (int i = 0; i < Server.NPCs.Count; i++)
        {
            var npc = Server.NPCs[i];
            if (npc == null) continue;
            // npc.MovementHandler.AddToPath(new Location(npc.Location.X + 1, npc.Location.Y));
            npc.MovementHandler.Process();
        }

        /* Update */
        for (int i = 0; i < Server.Players.Length; i++)
        {
            if (Server.Players[i] == null) continue;
            PlayerUpdater.Update(Server.Players[i]);
            NPCUpdater.Update(Server.Players[i]);
        }

        /* Send data */
        for (int i = 0; i < Server.Players.Length; i++)
        {
            if (Server.Players[i] == null) continue;
            Server.Players[i].PlayerSession.FlushBufferedData();
        }

        /* Reset */
        for (int i = 0; i < Server.Players.Length; i++)
        {
            if (Server.Players[i] == null) continue;
            Server.Players[i].Reset();
        }

        for (int i = 0; i < Server.NPCs.Count; i++)
        {
            if (Server.NPCs[i] == null) continue;
            Server.NPCs[i].Reset();
        }
    }
}