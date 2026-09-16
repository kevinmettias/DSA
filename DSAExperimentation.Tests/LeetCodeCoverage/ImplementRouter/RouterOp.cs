using DSAExperimentation.LeetCode.ImplementRouter;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementRouter;

// One call in a Router script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script
// (like ImplementRouterTests.Examples) reads like the LeetCode call sequence it
// replays. Its own file because ImplementRouterTests' public Theory signatures
// name it, so it cannot be a nested private helper - and a second file-level
// type beside the test class would leave the file's name identifying neither.
public readonly record struct RouterOp(RouterOp.OpKind kind, int first, int second, int third)
{
    public static RouterOp AddPacket(int source, int destination, int timestamp) =>
        new(OpKind.AddPacket, source, destination, timestamp);

    public static RouterOp ForwardPacket() => new(OpKind.ForwardPacket, 0, 0, 0);

    public static RouterOp GetCount(int destination, int startTime, int endTime) =>
        new(OpKind.GetCount, destination, startTime, endTime);

    // bool for addPacket, int[] for forwardPacket, int for getCount - boxed
    // uniformly so a script runner can assert against one expected value per
    // operation regardless of which method it dispatches to. Internal, not
    // public: IRouterStrategy is internal to ImplementRouterSolution, and only
    // this same assembly's ImplementRouterTests.RunScript ever calls Apply.
    internal object? Apply(ImplementRouterSolution.IRouterStrategy strategy) => kind switch
    {
        OpKind.AddPacket => strategy.AddPacket(first, second, third),
        OpKind.ForwardPacket => strategy.ForwardPacket(),
        _ => strategy.GetCount(first, second, third),
    };

    public enum OpKind
    {
        AddPacket,
        ForwardPacket,
        GetCount,
    }
}
