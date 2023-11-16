﻿using RuneRebirth2005.Entities;
using RuneRebirth2005.Entities.Combat;

namespace RuneRebirth2005.NPCManagement;

public interface INPC
{
    public int Index { get; set; }
    public string Name { get; set; }
    public int ModelId { get; set; }
    public int CombatLevel { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int AttackAnimation { get; set; }
    public int BlockAnimation { get; set; }
    public int FallAnimation { get; set; }
    public int CurrentAnimation { get; set; }
    public bool IsUpdateRequired { get; set; }
    public int InteractingEntityId { get; set; }
    public int Size { get; set; }
    public bool Stationary { get; set; }
    public IPlayer PlayerCombatFocus { get; set; }
    public INPC NPCCombatFocus { get; set; }
    public DamageInformation RecentDamageReceived { get; set; }
    public bool InCombat { get; set; }
    public bool ShouldRender { get; set; }
    public Location CurrentLocation { get; set; }
    public Location SpawnLocation { get; set; }
    public int AttackSpeed { get; set; }
    public CombatType CombatType { get; set; }
    public void Attack();
    public int CurrentTick { get; set; }
    public void SetCombatAnimation();
    public bool PerformedHit { get; set; }
    public NPCUpdateFlags Flags { get; set; }
    public Face Face { get; set; }

}