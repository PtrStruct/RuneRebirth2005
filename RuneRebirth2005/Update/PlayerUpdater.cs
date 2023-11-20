using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;
using RuneRebirth2005.Network;
using Serilog;

namespace RuneRebirth2005.Update;

public static class PlayerUpdater
{
    private static Player _player;
    private static RSStream PlayerFlagUpdateBlock { get; set; }

    public static void Update(Player player)
    {
        _player = player;
        PlayerFlagUpdateBlock = new RSStream(new byte[5000]);
        UpdateCurrentPlayerMovement(_player);

        if (_player.IsUpdateRequired || _player.IsAppearanceUpdate)
            UpdatePlayerState(_player, PlayerFlagUpdateBlock);

        UpdateLocalPlayers();
        AddPlayersToLocalList();

        Finalize(PlayerFlagUpdateBlock);
        _player.PlayerSession.Writer.EndFrameVarSizeWord();
    }

    private static void AddPlayersToLocalList()
    {
        for (int i = 0; i < Server.Players.Length; i++)
        {
            var player = Server.Players[i];
            if (player == null) continue;

            if (player.Index == -1 || player.Index == _player.Index) continue;

            if (!_player.LocalPlayers.Contains(player) && player.Location.IsWithinArea(_player.Location))
            {
                /* In order to render the local player */
                player.Flags |= PlayerUpdateFlags.Appearance;
                _player.LocalPlayers.Add(player);
                AddLocalPlayer(_player.PlayerSession.Writer, _player, player);
                UpdatePlayerState(player, PlayerFlagUpdateBlock);
            }
        }

        /* Finished adding local players */
        _player.PlayerSession.Writer.WriteBits(11, 2047);
    }

    private static void Finalize(RSStream PlayerFlagUpdateBlock)
    {
        if (PlayerFlagUpdateBlock.CurrentOffset > 0)
        {
            _player.PlayerSession.Writer.FinishBitAccess();
            _player.PlayerSession.Writer.WriteBytes(PlayerFlagUpdateBlock.Buffer, PlayerFlagUpdateBlock.CurrentOffset,
                0);
        }
        else
        {
            _player.PlayerSession.Writer.FinishBitAccess();
        }
    }

    static void AddLocalPlayer(RSStream writer, Player player, Player other)
    {
        writer.WriteBits(11, other.Index);
        writer.WriteBits(1, 1); /* Observed */
        writer.WriteBits(1, 1); /* Teleported */

        var delta = Location.Delta(player.Location, other.Location);
        writer.WriteBits(5, delta.Y);
        writer.WriteBits(5, delta.X);
        Log.Warning($"Adding: {other.Index} For {player.Index} DY: {other.Location.Y} - DX: {other.Location.X}");
    }

    private static void UpdateLocalPlayers()
    {
        _player.PlayerSession.Writer.WriteBits(8, _player.LocalPlayers.Count); // number of players to add

        foreach (var other in _player.LocalPlayers.ToList())
        {
            if (other.Location.IsWithinArea(_player.Location))
            {
                UpdateLocalPlayerMovement(other, _player.PlayerSession.Writer);

                if (other.IsUpdateRequired)
                {
                    other.Flags |= PlayerUpdateFlags.Appearance;
                    UpdatePlayerState(other, PlayerFlagUpdateBlock);
                }
            }
            else
            {
                RemovePlayer(other);
            }
        }
    }

    private static void UpdateLocalPlayerMovement(Player player, RSStream writer)
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

    private static void RemovePlayer(Player other)
    {
        _player.PlayerSession.Writer.WriteBits(1, 0);
        _player.LocalPlayers.Remove(other);
    }

    private static void UpdatePlayerState(Player player, RSStream playerFlagUpdateBlock)
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
        if ((mask & PlayerUpdateFlags.Animation) != 0) AppendAnimation(player, playerFlagUpdateBlock);
        // if ((mask & PlayerUpdateFlags.InteractingEntity) != 0) AppendInteractingEntity(player, updatetempBlock);
        if ((mask & PlayerUpdateFlags.InteractingEntity) != 0) AppendNPCInteract(player, playerFlagUpdateBlock);
        if ((mask & PlayerUpdateFlags.Appearance) != 0) AppendAppearance(player, playerFlagUpdateBlock);
        //if ((mask & PlayerUpdateFlags.FaceDirection) != 0) AppendFaceDirection(player, updatetempBlock);
        // if ((mask & PlayerUpdateFlags.SingleHit) != 0) AppendSingleHit(player, playerFlagUpdateBlock);
    }

    private static void AppendAnimation(Player player, RSStream playerFlagUpdateBlock)
    {
        playerFlagUpdateBlock.WriteWordBigEndian(player.CurrentAnimation);
        playerFlagUpdateBlock.WriteByte(0); //delay
    }


    private static void AppendNPCInteract(Player player, RSStream updatetempBlock)
    {
        updatetempBlock.WriteWordBigEndian(player.InteractingEntity.Index);
    }

    private static void AppendAppearance(Player player, RSStream playerFlagUpdateBlock)
    {
        var updateBlockBuffer = new RSStream(new byte[256]);
        updateBlockBuffer.WriteByte(player.Gender);
        updateBlockBuffer.WriteByte(player.HeadIcon); // Skull Icon

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
        WriteMovementAnimations(updateBlockBuffer, player);

        updateBlockBuffer.WriteQWord(player.Username.ToLong());
        updateBlockBuffer.WriteByte(70);
        updateBlockBuffer.WriteWord(player.TotalLevel);

        playerFlagUpdateBlock.WriteByteC(updateBlockBuffer.CurrentOffset);
        playerFlagUpdateBlock.WriteBytes(updateBlockBuffer.Buffer, updateBlockBuffer.CurrentOffset, 0);
    }

    private static void UpdateCurrentPlayerMovement(Player player)
    {
        player.PlayerSession.Writer.CreateFrameVarSizeWord(ServerOpCodes.PLAYER_UPDATE);
        player.PlayerSession.Writer.InitBitAccess();

        /* Idle */
        if (!player.IsUpdateRequired)
        {
            AppendIdleStand(player);
            return;
        }

        if (player.IsUpdateRequired)
        {
            AppendTeleportOrSpawn(player);
        }
        else
        {
            AppendMove();
        }
    }

    private static void AppendIdleStand(Player player)
    {
        player.PlayerSession.Writer.WriteBits(1, 0);
    }

    private static void AppendTeleportOrSpawn(Player player)
    {
        player.PlayerSession.Writer.WriteBits(1, 1); // set to true if updating thisPlayer
        player.PlayerSession.Writer.WriteBits(2, 3); // updateType - 3=jump to pos

        // the following applies to type 3 only
        player.PlayerSession.Writer.WriteBits(2, 0); // height level (0-3)
        player.PlayerSession.Writer.WriteBits(1, 1); // set to true, if discarding walking queue (after teleport e.g.)
        player.PlayerSession.Writer.WriteBits(1,
            player.IsUpdateRequired ? 1 : 0); // UpdateRequired aka does come with UpdateFlags
        player.PlayerSession.Writer.WriteBits(7, player.Location.PositionRelativeToOffsetChunkY); // y-position
        player.PlayerSession.Writer.WriteBits(7, player.Location.PositionRelativeToOffsetChunkX); // x-position
    }

    private static void AppendMove()
    {
    }

    private static void WriteBeard(RSStream stream, Player player)
    {
        var beard = player.Appearance.Beard;

        if (beard != 1 && GameConstants.IsFullHelm(player.Equipment.GetItem(EquipmentSlot.Helmet).ItemId) ||
            GameConstants.IsFullMask(player.Equipment.GetItem(EquipmentSlot.Helmet).ItemId))
            stream.WriteWord(0x100 + beard);
        else
            stream.WriteByte(0);
    }

    private static void WriteFeet(RSStream stream, Player player)
    {
        var itemId = player.Equipment.GetItem(EquipmentSlot.Boots).ItemId;
        var feetId = player.Appearance.Feet;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + feetId);
    }

    private static void WriteHands(RSStream stream, Player player)
    {
        var itemId = player.Equipment.GetItem(EquipmentSlot.Gloves).ItemId;
        var handsId = player.Appearance.Hands;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + handsId);
    }

    private static void WriteHair(RSStream stream, Player player)
    {
        var isFullHelmOrMask = GameConstants.IsFullHelm(player.Equipment.GetItem(EquipmentSlot.Helmet).ItemId) ||
                               GameConstants.IsFullMask(player.Equipment.GetItem(EquipmentSlot.Helmet).ItemId);
        if (!isFullHelmOrMask)
        {
            var hair = player.Appearance.Hair;
            stream.WriteWord(0x100 + hair);
        }
        else
            stream.WriteByte(0);
    }

    private static void WriteLegs(RSStream stream, Player player)
    {
        var itemId = player.Equipment.GetItem(EquipmentSlot.Legs).ItemId;
        var legsId = player.Appearance.Legs;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + legsId);
    }

    private static void WriteShield(RSStream stream, Player player)
    {
        var itemId = player.Equipment.GetItem(EquipmentSlot.Shield).ItemId;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private static void WriteBody(RSStream stream, Player player)
    {
        var itemId = player.Equipment.GetItem(EquipmentSlot.Chest).ItemId;
        var torsoId = player.Appearance.Torso;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + torsoId);
    }

    private static void WriteWeapon(RSStream stream, Player player)
    {
        var itemId = player.Equipment.GetItem(EquipmentSlot.Weapon).ItemId;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private static void WriteAmulet(RSStream stream, Player player)
    {
        var itemId = player.Equipment.GetItem(EquipmentSlot.Amulet).ItemId;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private static void WriteCape(RSStream stream, Player player)
    {
        var itemId = player.Equipment.GetItem(EquipmentSlot.Cape).ItemId;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private static void WriteHelmet(RSStream stream, Player player)
    {
        var itemId = player.Equipment.GetItem(EquipmentSlot.Helmet).ItemId;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private static void WriteArms(RSStream stream, Player player)
    {
        var isFullBody = GameConstants.IsFullBody(player.Equipment.GetItem(EquipmentSlot.Chest).ItemId);
        if (!isFullBody)
        {
            var arms = player.Appearance.Arms;
            stream.WriteWord(0x100 + arms);
        }
        else
            stream.WriteByte(0);
    }

    private static void WritePlayerColors(RSStream stream, Player player)
    {
        for (int i = 0; i < 5; i++)
        {
            stream.WriteByte(player.Colors.GetColors()[i]);
        }
    }

    private static void WriteMovementAnimations(RSStream stream, Player player)
    {
        foreach (var animation in player.MovementAnimations.GetAnimations())
            stream.WriteWord(animation);
    }
}