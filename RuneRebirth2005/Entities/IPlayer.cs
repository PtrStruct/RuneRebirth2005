using RuneRebirth2005.ClientManagement;
using RuneRebirth2005.Entities.Combat;
using RuneRebirth2005.NPCManagement;

namespace RuneRebirth2005.Entities;

public interface IPlayer
{
    public int Index { get; set; }
    public IPlayer PlayerCombatFocus { get; set; }
    public INPC NPCCombatFocus { get; set; }
    public DamageInformation RecentDamageInformation { get; set; }
    public int CurrentAnimation { get; set; }
    public int AttackAnimation { get; set; }
    public int BlockAnimation { get; set; }
    public int FallAnimation { get; set; }
    public bool IsUpdateRequired { get; set; }
    public CombatType AttackType { get; set; }
    public bool InCombat { get; set; }
    public int InteractingEntityId { get; set; }
    public void Attack();
    public PlayerUpdateFlags Flags { get; set; }
    public Client.PlayerData Data { get; set; }
    public Weapon Weapon { get; set; }
}