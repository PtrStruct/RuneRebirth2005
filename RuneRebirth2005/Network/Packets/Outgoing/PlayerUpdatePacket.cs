using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;
using Serilog;

namespace RuneRebirth2005.Network.Outgoing;

public class PlayerUpdatePacket
{
    private readonly Player _currentPlayer;

    public PlayerUpdatePacket(Player currentPlayer)
    {
        _currentPlayer = currentPlayer;
    }
    
    public RSStream PlayerFlagUpdateBlock { get; set; }

    public void Add()
    {
        PlayerFlagUpdateBlock = new RSStream(new byte[5000]);
        UpdateCurrentPlayerMovement();

        if (_currentPlayer.IsUpdateRequired)
            UpdatePlayerState(_currentPlayer, PlayerFlagUpdateBlock);

        UpdateLocalPlayers();
        AddPlayersToLocalList();

        Finalize(PlayerFlagUpdateBlock);
        _currentPlayer.Writer.EndFrameVarSizeWord();
    }


    private void Finalize(RSStream PlayerFlagUpdateBlock)
    {
        if (PlayerFlagUpdateBlock.CurrentOffset > 0)
        {
            _currentPlayer.Writer.FinishBitAccess();
            _currentPlayer.Writer.WriteBytes(PlayerFlagUpdateBlock.Buffer, PlayerFlagUpdateBlock.CurrentOffset, 0);
        }
        else
        {
            _currentPlayer.Writer.FinishBitAccess();
        }
    }

    private void AddPlayersToLocalList()
    {
        foreach (var player in Server.Players)
        {
            if (player.Index == -1 || player.Index == _currentPlayer.Index) continue;

            if (!_currentPlayer.LocalPlayers.Contains(player) && player.Data.Location.IsWithinArea(_currentPlayer.Data.Location))
            {
                _currentPlayer.LocalPlayers.Add(player);
                AddLocalPlayer(_currentPlayer.Writer, _currentPlayer, player);
                UpdatePlayerState(player, PlayerFlagUpdateBlock);
            }
        }

        /* Finished adding local players */
        _currentPlayer.Writer.WriteBits(11, 2047);
    }

    private void UpdateLocalPlayers()
    {
        _currentPlayer.Writer.WriteBits(8, _currentPlayer.LocalPlayers.Count); // number of players to add

        foreach (var other in _currentPlayer.LocalPlayers.ToList())
        {
            if (other.Data.Location.IsWithinArea(_currentPlayer.Data.Location) && !other.DidTeleportOrSpawn)
            {
                UpdateLocalPlayerMovement(other, _currentPlayer.Writer);

                if (other.IsUpdateRequired)
                    UpdatePlayerState(other, PlayerFlagUpdateBlock);
            }
            else
            {
                RemovePlayer(other);
            }
        }
    }

    private void UpdateLocalPlayerMovement(Player player, RSStream writer)
    {
        // var updateRequired = player.IsUpdateRequired;
        // var updateRequired = player.IsUpdateRequired;
        // var pDir = player.MovementHandler.PrimaryDirection;
        // var sDir = player.MovementHandler.SecondaryDirection;
        // if (pDir != -1)
        // {
        //     writer.WriteBits(1, 1);
        //     if (sDir != -1)
        //         AppendRun(writer, pDir, sDir, updateRequired);
        //     else
        //         AppendWalk(writer, pDir, updateRequired);
        // }
        // else
        // {
        //     if (updateRequired)
        //     {
        //         writer.WriteBits(1, 1);
        //         AppendStand(writer);
        //     }
        //     else
        //     {
        //         writer.WriteBits(1, 0);
        //     }
        // }

        // writer.WriteBits(1, 0);

        if (player.IsUpdateRequired)
        {
            writer.WriteBits(1, 1);
            writer.WriteBits(2, 0);
        }
        else
        {
            writer.WriteBits(1, 0);
        }
    }

    private void RemovePlayer(Player other)
    {
        _currentPlayer.Writer.WriteBits(1, 0);
        _currentPlayer.LocalPlayers.Remove(other);
    }

    private void UpdatePlayerState(Player player, RSStream playerFlagUpdateBlock)
    {
        PlayerUpdateFlags mask = player.Flags;

        if (mask >= PlayerUpdateFlags.FullMask)
        {
            mask |= PlayerUpdateFlags.FullMask;
            playerFlagUpdateBlock.WriteWordBigEndian((int)mask);
        }
        else
        {
            playerFlagUpdateBlock.WriteByte((byte)mask);
        }

        //if ((mask & PlayerUpdateFlags.Graphics) != 0) AppendGraphics(player, updatetempBlock);
        // if ((mask & PlayerUpdateFlags.Animation) != 0) AppendAnimation(player, updatetempBlock, player.AnimationId);
        // if ((mask & PlayerUpdateFlags.InteractingEntity) != 0) AppendInteractingEntity(player, updatetempBlock);
        //if ((mask & PlayerUpdateFlags.InteractingEntity) != 0) AppendNPCInteract(player, updatetempBlock);
        if ((mask & PlayerUpdateFlags.Appearance) != 0) AppendAppearance(player, playerFlagUpdateBlock);
        //if ((mask & PlayerUpdateFlags.FaceDirection) != 0) AppendFaceDirection(player, updatetempBlock);
        // if ((mask & PlayerUpdateFlags.SingleHit) != 0) AppendSingleHit(player, updatetempBlock);
    }

    private void AppendAppearance(Player player, RSStream playerFlagUpdateBlock)
    {
        var updateBlockBuffer = new RSStream(new byte[256]);
        updateBlockBuffer.WriteByte(player.Data.Gender);
        updateBlockBuffer.WriteByte(player.Data.HeadIcon); // Skull Icon
        
        WriteHelmet(updateBlockBuffer, player);
        WriteCape(updateBlockBuffer, player);
        WriteAmulet(updateBlockBuffer, player);
        WriteWeapon(updateBlockBuffer, player);
        WriteBody(updateBlockBuffer, player);
        WriteShield(updateBlockBuffer, player);
        WriteArms(updateBlockBuffer, player);
        WriteLegs(updateBlockBuffer, player);

        WriteHair(updateBlockBuffer, player);
        WriteHands(updateBlockBuffer, player);
        WriteFeet(updateBlockBuffer, player);
        WriteBeard(updateBlockBuffer, player);
        
        WritePlayerColors(updateBlockBuffer, player);
        WriteMovementAnimations(updateBlockBuffer);

        updateBlockBuffer.WriteQWord(player.Data.Username.ToLong());
        updateBlockBuffer.WriteByte(player.Data.CombatLevel);
        updateBlockBuffer.WriteWord(player.Data.TotalLevel);

        playerFlagUpdateBlock.WriteByteC(updateBlockBuffer.CurrentOffset);
        playerFlagUpdateBlock.WriteBytes(updateBlockBuffer.Buffer, updateBlockBuffer.CurrentOffset, 0);
    }

    void UpdateCurrentPlayerMovement()
    {
        _currentPlayer.Writer.CreateFrameVarSizeWord(ServerOpCodes.PLAYER_UPDATE);
        _currentPlayer.Writer.InitBitAccess();

        /* Idle */
        if (!_currentPlayer.IsUpdateRequired && !_currentPlayer.DidTeleportOrSpawn)
        {
            AppendIdleStand();
            return;
        }


        if (_currentPlayer.DidTeleportOrSpawn || _currentPlayer.IsUpdateRequired)
        {
            AppendTeleportOrSpawn();
        }
        else
        {
            AppendMove();
            /* If pDir != -1 then we're moving */
            /* Check if sDir != -1 then we're not only moving but we're running */
        }
    }

    private void AppendMove()
    {
    }

    private void AppendTeleportOrSpawn()
    {
        _currentPlayer.Writer.WriteBits(1, 1); // set to true if updating thisPlayer
        _currentPlayer.Writer.WriteBits(2, 3); // updateType - 3=jump to pos

        // the following applies to type 3 only
        _currentPlayer.Writer.WriteBits(2, 0); // height level (0-3)
        _currentPlayer.Writer.WriteBits(1, 1); // set to true, if discarding walking queue (after teleport e.g.)
        _currentPlayer.Writer.WriteBits(1,
            _currentPlayer.IsUpdateRequired ? 1 : 0); // UpdateRequired aka does come with UpdateFlags
        _currentPlayer.Writer.WriteBits(7, _currentPlayer.Data.Location.PositionRelativeToOffsetChunkY); // y-position
        _currentPlayer.Writer.WriteBits(7, _currentPlayer.Data.Location.PositionRelativeToOffsetChunkX); // x-position
        _currentPlayer.DidTeleportOrSpawn = false;
    }

    private void AppendIdleStand()
    {
        _currentPlayer.Writer.WriteBits(1, 0);
    }


    void AddLocalPlayer(RSStream writer, Player player, Player other)
    {
        writer.WriteBits(11, other.Index);
        writer.WriteBits(1, 1); /* Observed */
        writer.WriteBits(1, 1); /* Teleported */

        var delta = Location.Delta(player.Data.Location, other.Data.Location);
        writer.WriteBits(5, delta.Y);
        writer.WriteBits(5, delta.X);
        Log.Warning($"Adding: {other.Index} For {player.Index} DY: {other.Data.Location.Y} - DX: {other.Data.Location.X}");
    }

    private void WriteBeard(RSStream stream, Player client)
    {
        var beard = client.Data.Appearance.Beard;

        if (beard != 1 && GameConstants.IsFullHelm(client.Data.Equipment.GetItem(EquipmentSlot.Helmet).ItemId) ||
            GameConstants.IsFullMask(client.Data.Equipment.GetItem(EquipmentSlot.Helmet).ItemId))
            stream.WriteWord(0x100 + beard);
        else
            stream.WriteByte(0);
    }

    private void WriteFeet(RSStream stream, Player client)
    {
        var itemId = client.Data.Equipment.GetItem(EquipmentSlot.Boots).ItemId;
        var feetId = client.Data.Appearance.Feet;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + feetId);
    }

    private void WriteHands(RSStream stream, Player client)
    {
        var itemId = client.Data.Equipment.GetItem(EquipmentSlot.Gloves).ItemId;
        var handsId = client.Data.Appearance.Hands;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + handsId);
    }

    private void WriteHair(RSStream stream, Player client)
    {
        var isFullHelmOrMask = GameConstants.IsFullHelm(client.Data.Equipment.GetItem(EquipmentSlot.Helmet).ItemId) ||
                               GameConstants.IsFullMask(client.Data.Equipment.GetItem(EquipmentSlot.Helmet).ItemId);
        if (!isFullHelmOrMask)
        {
            var hair = client.Data.Appearance.Hair;
            stream.WriteWord(0x100 + hair);
        }
        else
            stream.WriteByte(0);
    }

    private void WriteLegs(RSStream stream, Player client)
    {
        var itemId = client.Data.Equipment.GetItem(EquipmentSlot.Legs).ItemId;
        var legsId = client.Data.Appearance.Legs;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + legsId);
    }

    private void WriteShield(RSStream stream, Player client)
    {
        var itemId = client.Data.Equipment.GetItem(EquipmentSlot.Shield).ItemId;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private void WriteBody(RSStream stream, Player client)
    {
        var itemId = client.Data.Equipment.GetItem(EquipmentSlot.Chest).ItemId;
        var torsoId = client.Data.Appearance.Torso;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + torsoId);
    }

    private void WriteWeapon(RSStream stream, Player client)
    {
        var itemId = client.Data.Equipment.GetItem(EquipmentSlot.Weapon).ItemId;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private void WriteAmulet(RSStream stream, Player client)
    {
        var itemId = client.Data.Equipment.GetItem(EquipmentSlot.Amulet).ItemId;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private void WriteCape(RSStream stream, Player client)
    {
        var itemId = client.Data.Equipment.GetItem(EquipmentSlot.Cape).ItemId;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private void WriteHelmet(RSStream stream, Player client)
    {
        var itemId = client.Data.Equipment.GetItem(EquipmentSlot.Helmet).ItemId;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private void WriteArms(RSStream stream, Player client)
    {
        var isFullBody = GameConstants.IsFullBody(client.Data.Equipment.GetItem(EquipmentSlot.Chest).ItemId);
        if (!isFullBody)
        {
            var arms = client.Data.Appearance.Arms;
            stream.WriteWord(0x100 + arms);
        }
        else
            stream.WriteByte(0);
    }

    private void WritePlayerColors(RSStream stream, Player client)
    {
        for (int i = 0; i < 5; i++)
        {
            stream.WriteByte(client.Data.Colors.GetColors()[i]);
        }
    }

    private void WriteMovementAnimations(RSStream stream)
    {
        foreach (var animation in _currentPlayer.Data.MovementAnimations.GetAnimations())
            stream.WriteWord(animation);
    }
}