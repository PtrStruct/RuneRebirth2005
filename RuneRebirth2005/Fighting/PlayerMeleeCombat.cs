using RuneRebirth2005.Entities;
using RuneRebirth2005.NPCManagement;
using Serilog;

namespace RuneRebirth2005.Fighting;

public class PlayerMeleeCombat
{
    private readonly Character _currentPlayer;

    private Character Target;
    public int CurrentTick { get; set; }
    public bool PerformedHit { get; set; }

    public PlayerMeleeCombat(Character currentPlayer)
    {
        _currentPlayer = currentPlayer;
        // CurrentTick = _currentPlayer.Weapon.Speed;
    }

    public void Attack()
    {
        // CurrentTick++;
        //
        // var PlayerFocus = _currentPlayer.PlayerCombatFocus;
        // var NPCFocus = _currentPlayer.NPCCombatFocus;
        //
        // if (_currentPlayer.Index == -1) return;
        // if (CurrentTick < _currentPlayer.Weapon.Speed) return;
        // if (PlayerFocus == null && NPCFocus == null) return;
        // if (PlayerFocus != null && NPCFocus != null)
        //     Log.Fatal("Player Focus AND NPCFocus != null");
        //
        // /* Distance checks etc.. */
        //
        // /* Calculate damage */
        // int damage = 1;
        //
        // if (NPCFocus != null)
        // {
        //     _currentPlayer.InteractingEntityId = NPCFocus.Index;
        //
        //     if (!NPCFocus.InCombat)
        //     {
        //         NPCFocus.CurrentTick = 0;
        //         DelayedTaskHandler.RegisterTask(new DelayedAttackTask
        //         {
        //             RemainingTicks = 1,
        //             Task = () =>
        //             {
        //                 if (NPCFocus.PlayerCombatFocus == null)
        //                 {
        //                     NPCFocus.PlayerCombatFocus = _currentPlayer;
        //                 }
        //             }
        //         });
        //     }
        //
        //     /* Perform attack on NPC */
        //
        //     NPCFocus.InCombat = true;
        //     NPCFocus.RecentDamageReceived.HasBeenHit = true;
        //     NPCFocus.RecentDamageReceived.HitBy = _currentPlayer;
        //     NPCFocus.RecentDamageReceived.Amount = damage;
        //     NPCFocus.RecentDamageReceived.DamageType = damage > 0 ? DamageType.Damage : DamageType.Block;
        //     NPCFocus.Flags |= NPCUpdateFlags.SingleHit;
        //     NPCFocus.CurrentHealth -= damage;
        // }

        // if (PlayerFocus != null)
        // {
        //     _currentPlayer.InteractingEntityId = PlayerFocus.Index;
        //
        //     if (!PlayerFocus.InCombat)
        //     {
        //         DelayedTaskHandler.RegisterTask(new DelayedAttackTask
        //         {
        //             RemainingTicks = 1,
        //             Task = () =>
        //             {
        //                 if (PlayerFocus.PlayerCombatFocus == null)
        //                 {
        //                     PlayerFocus.PlayerCombatFocus = _currentPlayer;
        //                 }
        //             }
        //         });
        //     }
        //
        //     /* Perform attack on Player */
        //     PlayerFocus.RecentDamageInformation.HasBeenHit = true;
        //     PlayerFocus.RecentDamageInformation.HitBy = _currentPlayer;
        //     PlayerFocus.RecentDamageInformation.Amount = damage;
        //     PlayerFocus.RecentDamageInformation.DamageType = damage > 0 ? DamageType.Damage : DamageType.Block;
        //     PlayerFocus.Flags |= PlayerUpdateFlags.SingleHit;
        //     PlayerFocus.Data.CurrentHealth -= damage;
        // }

        // _currentPlayer.Flags |= PlayerUpdateFlags.InteractingEntity;
        // _currentPlayer.InCombat = true;
        // PerformedHit = true;
        // CurrentTick = 0;
    }

    public void SetCombatAnimation()
    {
        // if (_currentPlayer.Index == -1) return;
        //
        // if (PerformedHit || _currentPlayer.RecentDamageInformation.HasBeenHit)
        // {
        //     _currentPlayer.Flags |= PlayerUpdateFlags.Animation;
        //     PerformAnimationForEntity(_currentPlayer);
        //     _currentPlayer.IsUpdateRequired = true;
        // }
    }

    private void PerformAnimationForEntity(Character player)
    {
        // if (entity is Player player)
        // {
        //     if (player.Data.CurrentHealth <= 0)
        //     {
        //         entity.CurrentAnimation = entity.FallAnimation;
        //         entity.CombatFocus = null;
        //         entity.InCombat = false;
        //         entity.RecentDamageInformation.HitBy.InCombat = false;
        //         entity.RecentDamageInformation.HitBy.CombatFocus = null;
        //         return;
        //     }
        // }
        // else if (entity is NPC npc)
        // {
        //     if (npc.CurrentHealth <= 0)
        //     {
        //         entity.CurrentAnimation = entity.FallAnimation;
        //         entity.CombatFocus = null;
        //         entity.InCombat = false;
        //         entity.RecentDamageInformation.HitBy.InCombat = false;
        //         entity.RecentDamageInformation.HitBy.CombatFocus = null;
        //         DelayedTaskHandler.RegisterTask(new DelayedAttackTask
        //         {
        //             RemainingTicks = 2,
        //             Task = () =>
        //             {
        //                 npc.Alive = false;
        //                 DelayedTaskHandler.RegisterTask(new DelayedAttackTask
        //                 {
        //                     RemainingTicks = 5,
        //                     Task = () => { npc.Alive = true; }
        //                 });
        //             }
        //         });
        //         return;
        //     }
        // }

        // if (!PerformedHit && player.RecentDamageInformation.HasBeenHit)
        // {
        //     player.CurrentAnimation = player.BlockAnimation;
        // }
        // else if (PerformedHit)
        // {
        //     player.CurrentAnimation = player.AttackAnimation;
        // }
    }
}