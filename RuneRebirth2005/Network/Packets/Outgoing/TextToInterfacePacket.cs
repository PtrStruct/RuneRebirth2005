using RuneRebirth2005.Entities;

namespace RuneRebirth2005.Network.Outgoing;

public class TextToInterfacePacket
{
    private readonly Player _player;

    public TextToInterfacePacket(Player player)
    {
        _player = player;
    }

    public void Add(string _text, int _interfaceId)
    {
        _player.Writer.CreateFrameVarSizeWord(ServerOpCodes.INTF_TEXT_ADD);
        _player.Writer.WriteString(_text);
        _player.Writer.WriteWordA(_interfaceId);
        _player.Writer.EndFrameVarSizeWord();
    }
}