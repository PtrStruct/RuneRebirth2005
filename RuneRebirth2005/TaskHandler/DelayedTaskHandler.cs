namespace RuneRebirth2005;

public interface IDelayedTask
{
    public int RemainingTicks { get; set; }
    public Action Task { get; set; }
}

public class DelayedTaskHandler
{
    private static readonly List<IDelayedTask> _tasks = new();

    public static void RegisterTask(IDelayedTask task)
    {
        _tasks.Add(task);
    }

    public static void Tick()
    {
        for (var i = _tasks.Count - 1; i >= 0; --i)
        {
            var task = _tasks[i];
            if (--task.RemainingTicks > 0) continue;

            task.Task?.Invoke();
            _tasks.RemoveAt(i);
        }
    }
}