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
                if (_currentEntity is Player player)
                {
                    player.CombatFocus.RecentDamageInformation.HasBeenHit = true;
                    player.CombatFocus.RecentDamageInformation.HitBy = _currentEntity;
                    player.CombatFocus.RecentDamageInformation.Amount = 1;
                    PerformedHit = true;

                    player.Flags |= PlayerUpdateFlags.InteractingEntity;
                    player.InteractingEntityId = _currentEntity.CombatFocus.Index;


                    if (player.CombatFocus.CombatFocus == null)
                    {
                        player.CombatFocus.CombatFocus = _currentEntity;
                    }
                }
                else if (_currentEntity is NPC npc)
                {
                    npc.CombatFocus.RecentDamageInformation.HasBeenHit = true;
                    npc.CombatFocus.RecentDamageInformation.HitBy = _currentEntity;
                    npc.CombatFocus.RecentDamageInformation.Amount = 1;
                    PerformedHit = true;

                    npc.Flags |= NPCUpdateFlags.InteractingEntity;
                    npc.InteractingEntityId = _currentEntity.CombatFocus.Index + 32768;
                }

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
            if (_currentEntity.CombatFocus != null)
                entity.CurrentAnimation = entity.BlockAnimation;
        }
        else if (PerformedHit)
        {
            if (_currentEntity.CombatFocus != null)
                entity.CurrentAnimation = entity.AttackAnimation;
        }
    }
}