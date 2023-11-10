using System.Net.Sockets;
using RuneRebirth2005.Entities;
using RuneRebirth2005.Network;
using RuneRebirth2005.NPCManagement;
using Serilog;

namespace RuneRebirth2005.ClientManagement;

public class Client
{
    public Client()
    {
        LoginHandler = new LoginHandler(this);
    }

    public int Index { get; set; } = -1;
    public PlayerData Data { get; set; } = new();
    public RSStream Reader { get; set; }
    public RSStream Writer { get; set; }
    public TcpClient Socket { get; set; }
    public NetworkStream NetworkStream { get; set; }
    public SessionEncryption InEncryption { get; set; }
    public SessionEncryption OutEncryption { get; set; }
    public LoginHandler LoginHandler { get; set; }

    public List<Player> LocalPlayers { get; set; } = new();
    public List<NPC> LocalNPCs { get; set; } = new();

    public void Disconnect(string reason)
    {
        Server.Players[Index] = new Player();
        Socket.Close();
        Log.Information($"Client {Index} disconnected. Reason: {reason}");
    }

    public void FillStream(int count)
    {
        if (!NetworkStream.CanRead)
        {
            return;
        }
        try
        {
            Reader.CurrentOffset = 0;
            var bytes = NetworkStream.Read(Reader.Buffer, 0, count);

            if (bytes < count)
            {
                throw new IOException("we couldn't read enough bytes");
            }
        }
        catch (IOException ex)
        {
            Disconnect(ex.Message);
        }
        catch (ObjectDisposedException e)
        {
            Disconnect("The socket was unexpectedly closed. Exception message: " + e.Message);
        }
    }

    public void FlushBufferedData()
    {
        if (!NetworkStream.CanWrite)
        {
            return;
        }
        try
        {
            NetworkStream.Write(Writer.Buffer, 0, Writer.CurrentOffset);
            NetworkStream.Flush();
            //Log.Information($"Flushed {Writer.CurrentOffset} bytes of data.");
            Writer.CurrentOffset = 0;
        }
        catch (IOException ex)
        {
            Disconnect(ex.Message);
        }
        catch (ObjectDisposedException e)
        {
            Disconnect("The socket was unexpectedly closed. Exception message: " + e.Message);
        }
    }
    
    public class PlayerData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Gender { get; set; }
        public int HeadIcon { get; set; }
        public int CombatLevel { get; set; }
        public int TotalLevel { get; set; }
        public PlayerColors Colors { get; set; } = new();
        public PlayerEquipment Equipment { get; set; } = new();
        public PlayerSkills PlayerSkills { get; set; } = new();
        public PlayerAppearance Appearance { get; set; } = new();
        public MovementAnimations MovementAnimations { get; set; } = new();
        public Location Location { get; set; } = new(3200, 3200);
        public PlayerBonuses Bonuses { get; set; } = new();
    }
}