using RuneRebirth2005.Entities;
using RuneRebirth2005.Network.Incoming;
using Serilog;

namespace RuneRebirth2005.Network;

public static class PacketFactory
{
    public static IPacket CreateClientPacket(int opcode, dynamic parameters)
    {
        switch (opcode)
        {
            case 164:
                return new RegularWalkPacket(parameters);
            case 122:
                return new FirstOptionClickPacket(parameters);
            case 103:
                return new PlayerCommandPacket(parameters);
            case 72:
                return new AttackNPCPacket(parameters);
            case 41:
                return new EquipItemPacket(parameters);
            case 214:
                return new MoveInventoryItemPacket(parameters);
            default:
                Log.Error($"No packet class implementation for opcode {opcode}.");
                break;
        }

        return null;
    }
}

