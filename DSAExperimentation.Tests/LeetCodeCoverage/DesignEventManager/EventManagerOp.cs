using DSAExperimentation.LeetCode.DesignEventManager;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignEventManager;

// One call in an EventManager script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script
// (like Examples in DesignEventManagerTests) reads like the LeetCode call
// sequence it replays.
public readonly record struct EventManagerOp(EventManagerOp.OpKind kind, int eventId, int newPriority)
{
    public static EventManagerOp UpdatePriority(int eventId, int newPriority) =>
        new(OpKind.UpdatePriority, eventId, newPriority);

    public static EventManagerOp PollHighest() => new(OpKind.PollHighest, 0, 0);

    // null for UpdatePriority (void), the polled eventId for PollHighest - so a
    // script runner can assert against one expected value per operation
    // uniformly. Internal, not public: IEventManagerStrategy is internal to
    // DesignEventManagerSolution, and only this same assembly's RunScript ever
    // calls Apply.
    internal int? Apply(DesignEventManagerSolution.IEventManagerStrategy strategy)
    {
        if (kind == OpKind.UpdatePriority)
        {
            strategy.UpdatePriority(eventId, newPriority);
            return null;
        }

        return strategy.PollHighest();
    }

    public enum OpKind
    {
        UpdatePriority,
        PollHighest,
    }
}
