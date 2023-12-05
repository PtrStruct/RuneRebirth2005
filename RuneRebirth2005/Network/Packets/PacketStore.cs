namespace RuneRebirth2005.Network;

public class PacketStore
{
    public Dictionary<int, List<IPacket>> Packets = new Dictionary<int, List<IPacket>>();

    private Dictionary<int, int> _maxPacketCounts = new Dictionary<int, int>
    {
        { 122, 1 }, /* OpCode, How many to allow to be processed per tick */
        { 4, 7 },
        { 241, 7 },
        { 86, 7 },
        { 3, 7 },
        { 41, 7 },
        { 164, 5 },
        { 103, 7 },
        { 72, 7 },
        { 214, 7 }
    };

    public void AddPacket(int opCode, IPacket packet)
    {
        if (packet == null) return;

        if (!Packets.ContainsKey(opCode))
        {
            Packets[opCode] = new List<IPacket>();
        }

        Packets[opCode].Add(packet);
    }

    public void ProcessPackets()
    {
        foreach (var packetList in Packets.Values)
        {
            foreach (var packet in packetList.ToList())
            {
                if (packetList.Remove(packet))
                {
                    packet.Process();
                }
            }
        }
    }
}