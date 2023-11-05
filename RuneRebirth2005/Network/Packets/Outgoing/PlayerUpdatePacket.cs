using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;
using Serilog;

namespace RuneRebirth2005.Network.Outgoing;

public class PlayerUpdatePacket(Player currentPlayer)
{
    public RSStream PlayerFlagUpdateBlock { get; set; }

    public void Add()
    {
        PlayerFlagUpdateBlock = new RSStream(new byte[5000]);
        UpdateCurrentPlayerMovement();

        if (currentPlayer.IsUpdateRequired)
            UpdatePlayerState(currentPlayer, PlayerFlagUpdateBlock);

        UpdateLocalPlayers();
        AddPlayersToLocalList();

        Finalize(PlayerFlagUpdateBlock);
        currentPlayer.Writer.EndFrameVarSizeWord();
    }


    private void Finalize(RSStream PlayerFlagUpdateBlock)
    {
        if (PlayerFlagUpdateBlock.CurrentOffset > 0)
        {
            currentPlayer.Writer.FinishBitAccess();
            currentPlayer.Writer.WriteBytes(PlayerFlagUpdateBlock.Buffer, PlayerFlagUpdateBlock.CurrentOffset, 0);
        }
        else
        {
            currentPlayer.Writer.FinishBitAccess();
        }
    }

    private void AddPlayersToLocalList()
    {
        foreach (var player in Server.Players)
        {
            if (player.Index == -1 || player.Index == currentPlayer.Index) continue;

            if (!currentPlayer.LocalPlayers.Contains(player) && player.Location.IsWithinArea(currentPlayer.Location))
            {
                currentPlayer.LocalPlayers.Add(player);
                AddLocalPlayer(currentPlayer.Writer, currentPlayer, player);
                UpdatePlayerState(player, PlayerFlagUpdateBlock);
            }
        }

        /* Finished adding local players */
        currentPlayer.Writer.WriteBits(11, 2047);
    }

    private void UpdateLocalPlayers()
    {
        currentPlayer.Writer.WriteBits(8, currentPlayer.LocalPlayers.Count); // number of players to add

        foreach (var other in currentPlayer.LocalPlayers.ToList())
        {
            if (other.Location.IsWithinArea(currentPlayer.Location) && !other.DidTeleportOrSpawn)
            {
                UpdateLocalPlayerMovement(other, currentPlayer.Writer);

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

        // if (updateRequired)
        // {
        //     writer.WriteBits(1, 1);
        //     writer.WriteBits(2, 0);
        //     // AppendStand(writer);
        //     player.IsUpdateRequired = false;
        // }
        // else
        // {
        writer.WriteBits(1, 0);
        // }
    }

    private void RemovePlayer(Player other)
    {
        currentPlayer.Writer.WriteBits(1, 0);
        currentPlayer.LocalPlayers.Remove(other);
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
        var updateBlockBuffer = new RSStream(new byte[128]);
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
        WriteMovementAnimations(updateBlockBuffer);

        updateBlockBuffer.WriteQWord(player.Username.ToLong());
        updateBlockBuffer.WriteByte(player.CombatLevel);
        updateBlockBuffer.WriteWord(player.TotalLevel);

        playerFlagUpdateBlock.WriteByteC(updateBlockBuffer.CurrentOffset);
        playerFlagUpdateBlock.WriteBytes(updateBlockBuffer.Buffer, updateBlockBuffer.CurrentOffset, 0);
    }

    void UpdateCurrentPlayerMovement()
    {
        currentPlayer.Writer.CreateFrameVarSizeWord(ServerOpCodes.PLAYER_UPDATE);
        currentPlayer.Writer.InitBitAccess();

        /* Idle */
        if (!currentPlayer.IsUpdateRequired && !currentPlayer.DidTeleportOrSpawn)
        {
            AppendIdleStand();
            return;
        }

        if (currentPlayer.DidTeleportOrSpawn)
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
        currentPlayer.Writer.WriteBits(1, 1); // set to true if updating thisPlayer
        currentPlayer.Writer.WriteBits(2, 3); // updateType - 3=jump to pos

        // the following applies to type 3 only
        currentPlayer.Writer.WriteBits(2, 0); // height level (0-3)
        currentPlayer.Writer.WriteBits(1, 1); // set to true, if discarding walking queue (after teleport e.g.)
        currentPlayer.Writer.WriteBits(1,
            currentPlayer.IsUpdateRequired ? 1 : 0); // UpdateRequired aka does come with UpdateFlags
        currentPlayer.Writer.WriteBits(7, currentPlayer.Location.PositionRelativeToOffsetChunkY); // y-position
        currentPlayer.Writer.WriteBits(7, currentPlayer.Location.PositionRelativeToOffsetChunkX); // x-position
        currentPlayer.DidTeleportOrSpawn = false;
    }

    private void AppendIdleStand()
    {
        currentPlayer.Writer.WriteBits(1, 0);
    }


    void AddLocalPlayer(RSStream writer, Player player, Player other)
    {
        writer.WriteBits(11, other.Index);
        writer.WriteBits(1, 1); /* Observed */
        writer.WriteBits(1, 1); /* Teleported */

        var delta = Location.Delta(player.Location, other.Location);
        writer.WriteBits(5, delta.Y);
        writer.WriteBits(5, delta.X);
        Log.Warning($"Adding: {other.Index} For {player.Index} DY: {other.Location.Y} - DX: {other.Location.X}");
    }

    private void WriteBeard(RSStream stream, Player client)
    {
        var beard = client.Appearance.Beard;

        if (beard <= 0)
            stream.WriteByte(0);
        else
            stream.WriteWord(0x100 + beard);
    }

    private void WriteFeet(RSStream stream, Player client)
    {
        var itemId = client.Equipment.Boots;
        var feetId = client.Appearance.Feet;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + feetId);
    }

    private void WriteHands(RSStream stream, Player client)
    {
        var itemId = client.Equipment.Gloves;
        var handsId = client.Appearance.Hands;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + handsId);
    }

    private void WriteHair(RSStream stream, Player client)
    {
        var isFullHelmOrMask = GameConstants.IsFullHelm(client.Equipment.Helmet) ||
                               GameConstants.IsFullMask(client.Equipment.Helmet);
        if (!isFullHelmOrMask)
        {
            var hair = client.Appearance.Hair;
            stream.WriteWord(0x100 + hair);
        }
        else
            stream.WriteByte(0);
    }

    private void WriteLegs(RSStream stream, Player client)
    {
        var itemId = client.Equipment.Legs;
        var legsId = client.Appearance.Legs;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + legsId);
    }

    private void WriteShield(RSStream stream, Player client)
    {
        var itemId = client.Equipment.Shield;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private void WriteBody(RSStream stream, Player client)
    {
        var itemId = client.Equipment.Body;
        var torsoId = client.Appearance.Torso;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteWord(0x100 + torsoId);
    }

    private void WriteWeapon(RSStream stream, Player client)
    {
        var itemId = client.Equipment.Weapon;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private void WriteAmulet(RSStream stream, Player client)
    {
        var itemId = client.Equipment.Amulet;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private void WriteCape(RSStream stream, Player client)
    {
        var itemId = client.Equipment.Cape;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private void WriteHelmet(RSStream stream, Player client)
    {
        var itemId = client.Equipment.Helmet;
        if (itemId > -1)
            stream.WriteWord(0x200 + itemId);
        else
            stream.WriteByte(0);
    }

    private void WriteArms(RSStream stream, Player client)
    {
        var isFullBody = GameConstants.IsFullBody(client.Equipment.Body);
        if (!isFullBody)
        {
            var arms = client.Appearance.Arms;
            stream.WriteWord(0x100 + arms);
        }
        else
            stream.WriteByte(0);
    }

    private void WritePlayerColors(RSStream stream, Player client)
    {
        for (int i = 0; i < 5; i++)
        {
            stream.WriteByte(client.Colors.GetColors()[i]);
        }
    }

    private void WriteMovementAnimations(RSStream stream)
    {
        foreach (var animation in currentPlayer.MovementAnimations.GetAnimations())
            stream.WriteWord(animation);
    }
}