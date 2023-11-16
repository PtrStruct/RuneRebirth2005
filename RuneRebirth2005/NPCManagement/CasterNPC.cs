using RuneRebirth2005.Entities;
using RuneRebirth2005.Entities.Combat;
using Serilog;

namespace RuneRebirth2005.NPCManagement;

public class CasterNPC : INPC
{
    public int Index { get; set; }
    public string Name { get; set; }
    public int MaxHealth { get; set; }
    public int ModelId { get; set; }
    public int CombatLevel { get; set; }
    public int CurrentHealth { get; set; }
    public IPlayer CombatFocus { get; set; }
    public int CurrentAnimation { get; set; }
    public int AttackAnimation { get; set; }
    public int BlockAnimation { get; set; }
    public int FallAnimation { get; set; }
    public int Size { get; set; }
    public bool Stationary { get; set; }
    public bool ShouldRender { get; set; }
    public Location CurrentLocation { get; set; }
    public Location SpawnLocation { get; set; }
    public Location Location { get; set; }
    public CombatType CombatType { get; set; } = CombatType.Caster;
    public bool IsUpdateRequired { get; set; }
    public bool InCombat { get; set; }
    public NPCUpdateFlags Flags { get; set; }
    public int InteractingEntityId { get; set; }
    public IPlayer PlayerCombatFocus { get; set; }
    public INPC NPCCombatFocus { get; set; }
    public DamageInformation RecentDamageReceived { get; set; } = new();
    public bool PerformedHit { get; set; }
    public Face Face { get; set; }
    public int CurrentTick { get; set; }
    public int AttackSpeed { get; set; }


    public void Attack()
    {
        // /* Load GFX/Projectile */
        if (Index == -1) return;

        CurrentTick++;
        if (CurrentTick < AttackSpeed) return;
        if (!InCombat) return;


        var PlayerFocus = PlayerCombatFocus;
        var NPCFocus = NPCCombatFocus;

        if (Index == -1) return;
        if (CurrentTick < AttackSpeed) return;

        if (PlayerFocus == null && NPCFocus == null) return;
        if (PlayerFocus != null && NPCFocus != null)
            Log.Fatal("Player Focus AND NPCFocus != null");

        /* Calculate damage */
        int damage = 1;

        InteractingEntityId = PlayerFocus.Index + 32768;
        Flags |= NPCUpdateFlags.InteractingEntity;

        /* Perform attack on Player */
        PlayerFocus.RecentDamageInformation.HasBeenHit = true;
        PlayerFocus.RecentDamageInformation.HitByNPC = this;
        PlayerFocus.RecentDamageInformation.Amount = damage;
        PlayerFocus.RecentDamageInformation.DamageType = damage > 0 ? DamageType.Damage : DamageType.Block;
        PlayerFocus.Flags |= PlayerUpdateFlags.SingleHit;
        PlayerFocus.Data.CurrentHealth -= damage;

        InCombat = true;
        PerformedHit = true;
        CurrentTick = 0;
    }

    public void SetCombatAnimation()
    {
        if (Index == -1) return;

        if (InCombat)
        {
            if (!PerformedHit && RecentDamageReceived.HasBeenHit)
            {
                CurrentAnimation = BlockAnimation;
                Flags |= NPCUpdateFlags.Animation;
                IsUpdateRequired = true;
            }

            if (PerformedHit)
            {
                CurrentAnimation = AttackAnimation;
                Flags |= NPCUpdateFlags.Animation;
                IsUpdateRequired = true;
            }
        }
    }
}