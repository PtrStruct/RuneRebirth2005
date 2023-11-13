using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Entities.Combat;

public class MeleeCombat
{
    private readonly IEntity _currentEntity;

    private IEntity Target;
    public int CurrentTick { get; set; }
    public bool PerformedHit { get; set; }

    public MeleeCombat(IEntity currentEntity)
    {
        _currentEntity = currentEntity;
        CurrentTick = _currentEntity.Weapon.Speed;
    }

    public void CalculateAttack()
    {
        CurrentTick++;

        if (_currentEntity.Index == -1) return;

        if (CurrentTick >= _currentEntity.Weapon.Speed)
        {
            if (_currentEntity.CombatFocus != null)
            {
                _currentEntity.InCombat = true;

                if (_currentEntity is Player player)
                {
                    int damage = 1;
                    player.CombatFocus.RecentDamageInformation.HasBeenHit = true;
                    player.CombatFocus.RecentDamageInformation.HitBy = _currentEntity;
                    player.CombatFocus.RecentDamageInformation.Amount = damage;
                    player.CombatFocus.RecentDamageInformation.DamageType =
                        damage > 0 ? DamageType.Damage : DamageType.Block;


                    // if (player.CombatFocus is Player playerTarget)
                    // {
                    //     playerTarget.Flags |= PlayerUpdateFlags.SingleHit;
                    //     playerTarget.Data.CurrentHealth -= damage;
                    // }
                    // else if (player.CombatFocus is NPC NPCTarget)
                    // {
                    //     NPCTarget.Flags |= NPCUpdateFlags.SingleHit;
                    //     NPCTarget.CurrentHealth -= damage;
                    // }
                    //
                    if (player.CombatFocus is Player playerTarget)
                    {
                        playerTarget.Flags |= PlayerUpdateFlags.SingleHit;
                        playerTarget.Data.CurrentHealth -= damage;
                    }
                    else if (player.CombatFocus is NPC NPCTarget)
                    {
                        NPCTarget.Flags |= NPCUpdateFlags.SingleHit;
                        NPCTarget.CurrentHealth -= damage;
                    }


                    player.Flags |= PlayerUpdateFlags.InteractingEntity;
                    player.InteractingEntityId = _currentEntity.CombatFocus.Index;

                    /* If we've hit the target and it's not in combat */
                    if (!player.CombatFocus.InCombat)
                    {
                        DelayedTaskHandler.RegisterTask(new DelayedAttackTask
                        {
                            RemainingTicks = 1,
                            Task = () =>
                            {
                                if (player.CombatFocus.CombatFocus == null)
                                {
                                    player.CombatFocus.CombatFocus = _currentEntity;
                                }
                            }
                        });
                    }
                }
                else if (_currentEntity is NPC npc)
                {
                    int damage = 1;
                    npc.CombatFocus.RecentDamageInformation.HasBeenHit = true;
                    npc.CombatFocus.RecentDamageInformation.HitBy = _currentEntity;
                    npc.CombatFocus.RecentDamageInformation.Amount = 1;
                    npc.CombatFocus.RecentDamageInformation.DamageType =
                        damage > 0 ? DamageType.Damage : DamageType.Block;

                    PerformedHit = true;

                    if (npc.CombatFocus is Player playerTarget)
                    {
                        playerTarget.Flags |= PlayerUpdateFlags.SingleHit;
                        playerTarget.Data.CurrentHealth -= damage;
                    }
                    else if (npc.CombatFocus is NPC NPCTarget)
                    {
                        NPCTarget.Flags |= NPCUpdateFlags.SingleHit;
                        NPCTarget.CurrentHealth -= damage;
                    }

                    npc.Flags |= NPCUpdateFlags.InteractingEntity;

                    npc.InteractingEntityId = _currentEntity.CombatFocus.Index + 32768;
                }

                PerformedHit = true;
                CurrentTick = 0;
            }
        }
    }

    public void DecideCombatAnimation()
    {
        if (_currentEntity.Index == -1) return;

        if (PerformedHit || _currentEntity.RecentDamageInformation.HasBeenHit)
        {
            if (_currentEntity is Player player)
            {
                player.Flags |= PlayerUpdateFlags.Animation;
                PerformAnimationForEntity(player);
                player.IsUpdateRequired = true;
            }
            else if (_currentEntity is NPC npc)
            {
                npc.Flags |= NPCUpdateFlags.Animation;
                PerformAnimationForEntity(npc);
                npc.IsUpdateRequired = true;
            }
        }
    }

    private void PerformAnimationForEntity(IEntity entity)
    {
        if (!PerformedHit && entity.RecentDamageInformation.HasBeenHit)
        {
            entity.CurrentAnimation = entity.BlockAnimation;
        }
        else if (PerformedHit)
        {
            entity.CurrentAnimation = entity.AttackAnimation;
        }
    }
}