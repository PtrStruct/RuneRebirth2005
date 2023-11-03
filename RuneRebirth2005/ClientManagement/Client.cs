using System.Net.Sockets;
using RuneRebirth2005.Network;

namespace RuneRebirth2005.ClientManagement;

public class Client
{
    public Client()
    {
        LoginHandler = new LoginHandler(this);
    }

    public int Index { get; set; } = -1;
    public RSStream Reader { get; set; }
    public RSStream Writer { get; set; }
    public TcpClient Socket { get; set; }
    public NetworkStream NetworkStream { get; set; }
    public SessionEncryption InEncryption { get; set; }
    public SessionEncryption OutEncryption { get; set; }
    public LoginHandler LoginHandler { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public void Disconnect(string reason)
    {
        Server.Players[Index] = null;
        Socket.Close();
        Console.WriteLine($"Client {Index} disconnected. Reason: {reason}");
    }

    public void FillStream(int count)
    {
        try
        {
            Reader.CurrentOffset = 0;
            NetworkStream.Read(Reader.Buffer, 0, count);
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

    public void DirectFlushStream()
    {
        try
        {
            NetworkStream.Write(Writer.Buffer, 0, Writer.CurrentOffset);
            Console.WriteLine($"Flushing {Writer.CurrentOffset} bytes of data.");
            Writer.CurrentOffset = 0;
            NetworkStream.Flush();
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
}