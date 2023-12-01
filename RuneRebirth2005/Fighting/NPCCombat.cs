﻿using RuneRebirth2005.Entities;
using RuneRebirth2005.Helpers;

namespace RuneRebirth2005.Fighting;

public class NPCCombat : CombatBase
{
    private readonly NPC _character;

    public NPCCombat(NPC character) : base(character)
    {
        _character = character;
    }

    public override void DoAttack()
    {
        if (Target != null)
        {
            if (CanReach(Character, Target))
            {
                if (AttackTimer <= 0)
                {
                    /* Check if can combat */
                    if (CombatHelper.CanAttack(Character, Target))
                    {
                        Character.Combat.PerformedHit = true;
                        Target.Combat.WasHit = true;

                        Target.Combat.HitQueue.AddHit(new CombatHit
                        {
                            Damage = 1,
                            HitType = 1,
                            Attacker = Character,
                            Target = Target
                        }, true);
                        AttackTimer = Character.AttackSpeed;
                    }
                }
            }
        }
    }

    private bool CanReach(Character attacker, Character target)
    {
        var npc = Character as NPC;
        NpcHelper.npcs.TryGetValue(npc.ModelId, out var data);
        return attacker.Location.IsWithinDistance(target.Location, data.AttackDistance);
    }

    public override void PerformAnimation()
    {
        if (Character.Combat.Target?.CurrentHealth <= 0)
        {
            Reset();
        }

        if (!PerformedHit && WasHit)
        {
            Character.PerformAnimation(Character.BlockAnimation);
        }
        else if (PerformedHit)
        {
            Character.PerformAnimation(Character.AttackAnimation);
            if (Target is Player player)
            {
                var pX = player.Location.X;
                var pY = player.Location.Y;

                var nX = Character.Location.X;
                var nY = Character.Location.Y;

                var offX = (pY - nY) * -1;
                var offY = (pX - nX) * -1;

                var npc = Character as NPC;

                NpcHelper.npcs.TryGetValue(npc.ModelId, out var data);
                
                player.PacketSender.CreateProjectile(nX, nY, offX, offY, 50, 85, data.ProjectileId, 43, 31, (-player.Index) -1, 65);
            }
        }

        if (Character.CurrentHealth <= 0)
        {
            Character.PerformAnimation(Character.FallAnimation);
            Reset();

            var npc = Character as NPC;
            DelayedTaskHandler.RegisterTask(new DelayedAttackTask
            {
                RemainingTicks = 4,
                Task = () => { npc.Respawn(); }
            });
        }
    }
}