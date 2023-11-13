namespace RuneRebirth2005;

public class DelayedAttackTask : IDelayedTask
{
    public int RemainingTicks { get; set; }
    public Action Task { get; set; }
}