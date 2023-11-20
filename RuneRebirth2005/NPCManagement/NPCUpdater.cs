using RuneRebirth2005.Entities;
using RuneRebirth2005.Network;

namespace RuneRebirth2005.NPCManagement;

public static class NPCUpdater
{
    public static void Update(Player player)
    {
        if (player == null) return;

        var updateBlock = new RSStream(new byte[4096]);

        player.PlayerSession.Writer.CreateFrameVarSizeWord(ServerOpCodes.NPC_UPDATE);
        player.PlayerSession.Writer.InitBitAccess();

        player.PlayerSession.Writer.WriteBits(8, player.LocalNPCs.Count);

        foreach (var npc in player.LocalNPCs.ToList())
        {
            if (npc == null)
            {
                continue;
            }

            if (Server.NPCs[npc.Index] != null && player.Location.IsWithinArea(npc.Location) && npc.CurrentHealth > 0)
            {
                UpdateMovement(npc, player.PlayerSession.Writer);

                if (npc.IsUpdateRequired)
                    UpdateNPCState(npc, updateBlock);
            }
            else
            {
                //Console.WriteLine($"Removed: {npc.Name}");
                player.LocalNPCs.Remove(npc);
                player.PlayerSession.Writer.WriteBits(1, 1);
                player.PlayerSession.Writer.WriteBits(2, 3);
            }
        }

        foreach (var npc in Server.NPCs.Values)
        {
            if (player.LocalNPCs.Count >= 255)
                break;

            if (npc == null) continue;

            if (player.LocalNPCs.Contains(npc))
                continue;

            if (npc.CurrentHealth <= 0)
            {
                player.LocalNPCs.Remove(npc);
                continue;
            }

            if (npc.Location.IsWithinArea(player.Location))
            {
                // Console.WriteLine($"Added: {npc.ModelId}.");
                player.LocalNPCs.Add(npc);
                // npc.Flags |= NPCUpdateFlags.Face;
                // npc.IsUpdateRequired = true;

                AddNPC(player, npc, player.PlayerSession.Writer);
                if (npc.IsUpdateRequired)
                    UpdateNPCState(npc, updateBlock);

                // npc.NeedsPlacement = false;
            }
            else
            {
                //Console.WriteLine($"Removed: {npc.Name}");
                player.LocalNPCs.Remove(npc);
            }
        }

        if (updateBlock.CurrentOffset > 0)
        {
            player.PlayerSession.Writer.WriteBits(14, 16383);
            player.PlayerSession.Writer.FinishBitAccess();
            player.PlayerSession.Writer.WriteBytes(updateBlock.Buffer, updateBlock.CurrentOffset, 0);
        }
        else
        {
            player.PlayerSession.Writer.FinishBitAccess();
        }

        player.PlayerSession.Writer.EndFrameVarSizeWord();
        //client.FlushBufferedData();
    }

    private static void UpdateMovement(NPC npc, RSStream writer)
    {
        if (npc.IsUpdateRequired)
        {
            writer.WriteBits(1, 1);
            writer.WriteBits(2, 0);
        }
        else
        {
            writer.WriteBits(1, 0);
        }

        // if (npc.MovementHandler.SecondaryDirection == -1)
        // {
        //     if (npc.MovementHandler.PrimaryDirection == -1)
        //     {
        //         if (npc.IsUpdateRequired)
        //         {
        //             writer.WriteBits(1, 1);
        //             writer.WriteBits(2, 0);
        //         }
        //         else
        //         {
        //             writer.WriteBits(1, 0);
        //         }
        //     }
        //     else
        //     {
        //         writer.WriteBits(1, 1);
        //         writer.WriteBits(2, 1);
        //         writer.WriteBits(3, npc.MovementHandler.PrimaryDirection);
        //         writer.WriteBits(1, npc.IsUpdateRequired ? 1 : 0);
        //     }
        // }
        // else
        // {
        //     writer.WriteBits(1, 1);
        //     writer.WriteBits(2, 2);
        //     writer.WriteBits(3, npc.MovementHandler.PrimaryDirection);
        //     writer.WriteBits(3, npc.MovementHandler.SecondaryDirection);
        //     writer.WriteBits(1, npc.IsUpdateRequired ? 1 : 0);
        // }
    }

    private static void UpdateNPCState(NPC npc, RSStream updateBlock)
    {
        var mask = npc.Flags;
        //
        // // if (npc.Flags.HasFlag(NPCUpdateFlags.Animation))
        // // {
        // //     mask |= NPCUpdateFlags.Animation;
        // // }
        // //
        // // if (npc.Flags.HasFlag(NPCUpdateFlags.Graphics))
        // // {
        // //     mask |= NPCUpdateFlags.Graphics;
        // // }
        // //
        // // if (npc.Flags.HasFlag(NPCUpdateFlags.SingleHit))
        // // {
        // //     mask |= NPCUpdateFlags.SingleHit;
        // // }
        // //
        // // if (npc.Flags.HasFlag(NPCUpdateFlags.InteractingEntity))
        // // {
        // //     mask |= NPCUpdateFlags.InteractingEntity;
        // // }
        //
        // //if (npc.Flags.HasFlag(NPCUpdateFlags.Face))
        // //{
        // //    mask |= NPCUpdateFlags.Face;
        // //}
        //
        updateBlock.WriteByte((byte)mask);
        //
         if ((mask & NPCUpdateFlags.Animation) != 0)
         {
             updateBlock.WriteWordBigEndian(npc.CurrentAnimation);
             updateBlock.WriteByte(0); //delay
         }

        //
        // // if ((mask & NPCUpdateFlags.Graphics) != 0)
        // // {
        // //     updateBlock.WriteWordBigEndian(npc.GraphicsId);
        // //     updateBlock.WriteDWord(4);
        // // }
        // //
        // if ((mask & NPCUpdateFlags.SingleHit) != 0)
        // {
        //     updateBlock.WriteByteA((byte)npc.RecentDamageReceived.Amount); //hitDamage
        //     updateBlock.WriteByteC((byte)npc.RecentDamageReceived.DamageType); //hitType
        //     updateBlock.WriteByteA(npc.CurrentHealth); //currentHealth
        //     updateBlock.WriteByte(npc.MaxHealth); //maxHealth
        // }
        //
         if ((mask & NPCUpdateFlags.InteractingEntity) != 0)
         {
             var id = npc.InteractingEntity.Index + 32768;
             updateBlock.WriteWord(id);
         }

        if ((mask & NPCUpdateFlags.Face) != 0)
        {
            updateBlock.WriteWordBigEndian(npc.Face == null ? 0 : npc.Face.X);
            updateBlock.WriteWordBigEndian(npc.Face == null ? 0 : npc.Face.Y);
        }
    }


    private static void AddNPC(Player player, NPC npc, RSStream playerWriter)
    {
        playerWriter.WriteBits(14, npc.Index);
        playerWriter.WriteBits(5, npc.Location.Y - player.Location.Y);
        playerWriter.WriteBits(5, npc.Location.X - player.Location.X);
        playerWriter.WriteBits(1, 0);
        playerWriter.WriteBits(12, npc.ModelId);
        playerWriter.WriteBits(1, npc.IsUpdateRequired ? 1 : 0);
    }
}