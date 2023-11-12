using RuneRebirth2005.Entities;
using RuneRebirth2005.Network;

namespace RuneRebirth2005.NPCManagement;

public class NPCUpdater
{
    // public static void GenerateMovement()
    // {
    //     foreach (var npc in Server.NPCs)
    //     {
    //         if (npc == null)
    //             continue;
    //
    //         var shouldMove = new Random().Next(0, 3) == 0;
    //         var dir = MovementHelper.DirectionFromDelta(1, 1);
    //         if (shouldMove && npc.CanWalk)
    //         {
    //             npc.Location.Move(1,1);
    //             if (dir != -1)
    //                 npc.MovementHandler.PrimaryDirection = dir;
    //         }
    //     }
    // }

    public static void Update()
    {
        foreach (var player in Server.Players)
        {
            if (player.Index == -1) continue;

            var updateBlock = new RSStream(new byte[4096]);

            player.Writer.CreateFrameVarSizeWord(ServerOpCodes.NPC_UPDATE);
            player.Writer.InitBitAccess();

            player.Writer.WriteBits(8, player.LocalNPCs.Count);

            foreach (var npc in player.LocalNPCs.ToList())
            {
                if (npc == null)
                {
                    continue;
                }

                if (NPCManager.WorldNPCs[npc.Index] != null && player.Data.Location.IsWithinArea(npc.Location) &&
                    npc.Alive)
                {
                    UpdateMovement(npc, player.Writer);
                    
                    if (npc.IsUpdateRequired)
                        UpdateNPCState(npc, updateBlock);
                }
                else
                {
                    //Console.WriteLine($"Removed: {npc.Name}");
                    player.LocalNPCs.Remove(npc);
                    player.Writer.WriteBits(1, 1);
                    player.Writer.WriteBits(2, 3);
                }
            }

            foreach (var npc in NPCManager.WorldNPCs)
            {
                if (player.LocalNPCs.Count >= 255)
                    break;

                if (npc == null) continue;

                if (player.LocalNPCs.Contains(npc))
                    continue;

                if (!npc.Alive)
                {
                    player.LocalNPCs.Remove(npc);
                    continue;
                }

                if (npc.Location.IsWithinArea(player.Data.Location))
                {
                    // Console.WriteLine($"Added: {npc.ModelId}.");
                    player.LocalNPCs.Add(npc);
                    // npc.Flags |= NPCUpdateFlags.Face;
                    // npc.IsUpdateRequired = true;

                    AddNPC(player, npc, player.Writer);
                    if (npc.IsUpdateRequired)
                        UpdateNPCState(npc, updateBlock);

                    npc.NeedsPlacement = false;
                }
                else
                {
                    //Console.WriteLine($"Removed: {npc.Name}");
                    player.LocalNPCs.Remove(npc);
                }
            }

            if (updateBlock.CurrentOffset > 0)
            {
                player.Writer.WriteBits(14, 16383);
                player.Writer.FinishBitAccess();
                player.Writer.WriteBytes(updateBlock.Buffer, updateBlock.CurrentOffset, 0);
            }
            else
            {
                player.Writer.FinishBitAccess();
            }

            player.Writer.EndFrameVarSizeWord();
            //client.FlushBufferedData();
        }
    }

    public static void Reset()
    {
        foreach (var npc in NPCManager.WorldNPCs)
        {
            if (npc == null || npc.Index == -1) continue;

            npc.Flags = NPCUpdateFlags.None;
            npc.IsUpdateRequired = false;
            npc.NeedsPlacement = false;
            npc.CurrentAnimation = -1;
            npc.RecentDamageInformation.HasBeenHit = false;
            npc.MeleeCombat.PerformedHit = false;
            //npc.AnimationUpdateRequired = false;
            //npc.GraphicsUpdateRequired = false;
            //npc.SingleHitUpdateRequired = false;
            //npc.ForceChatUpdateRequired = false;
            //npc.TransformUpdateRequired = false;
            //npc.FaceUpdateRequired = false;
            //npc.IsUpdateRequired = false;

            //npc.MovementHandler.PrimaryDirection = -1;
            //npc.MovementHandler.SecondaryDirection = -1;
        }
    }


    private static void UpdateNPCState(NPC npc, RSStream updateBlock)
    {
        var mask = npc.Flags;

        // if (npc.Flags.HasFlag(NPCUpdateFlags.Animation))
        // {
        //     mask |= NPCUpdateFlags.Animation;
        // }
        //
        // if (npc.Flags.HasFlag(NPCUpdateFlags.Graphics))
        // {
        //     mask |= NPCUpdateFlags.Graphics;
        // }
        //
        // if (npc.Flags.HasFlag(NPCUpdateFlags.SingleHit))
        // {
        //     mask |= NPCUpdateFlags.SingleHit;
        // }
        //
        // if (npc.Flags.HasFlag(NPCUpdateFlags.InteractingEntity))
        // {
        //     mask |= NPCUpdateFlags.InteractingEntity;
        // }

        //if (npc.Flags.HasFlag(NPCUpdateFlags.Face))
        //{
        //    mask |= NPCUpdateFlags.Face;
        //}

        updateBlock.WriteByte((byte)mask);

        if ((mask & NPCUpdateFlags.Animation) != 0)
        {
            updateBlock.WriteWordBigEndian(npc.CurrentAnimation);
            updateBlock.WriteByte(0); //delay
        }

        // if ((mask & NPCUpdateFlags.Graphics) != 0)
        // {
        //     updateBlock.WriteWordBigEndian(npc.GraphicsId);
        //     updateBlock.WriteDWord(4);
        // }
        //
        // if ((mask & NPCUpdateFlags.SingleHit) != 0)
        // {
        //     updateBlock.WriteByteA((byte)npc.CombatHandler.CombatMethod.DamageInfo.Amount); //hitDamage
        //     updateBlock.WriteByteC((byte)npc.CombatHandler.CombatMethod.DamageInfo.Type); //hitType
        //     updateBlock.WriteByteA(npc.Health); //currentHealth
        //     updateBlock.WriteByte(npc.MaxHealth); //maxHealth
        // }
        //
         if ((mask & NPCUpdateFlags.InteractingEntity) != 0)
         {
             var id = npc.InteractingEntityId;
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
        playerWriter.WriteBits(5, npc.Location.Y - player.Data.Location.Y);
        playerWriter.WriteBits(5, npc.Location.X - player.Data.Location.X);
        playerWriter.WriteBits(1, 0);
        playerWriter.WriteBits(12, npc.ModelId);
        playerWriter.WriteBits(1, npc.IsUpdateRequired ? 1 : 0);
    }

    private static void UpdateMovement(NPC npc, RSStream writer)
    {
        if (npc.NeedsPlacement || npc.IsUpdateRequired)
        {
            writer.WriteBits(1, 1);
            writer.WriteBits(2, 0);
            npc.NeedsPlacement = false;
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
}