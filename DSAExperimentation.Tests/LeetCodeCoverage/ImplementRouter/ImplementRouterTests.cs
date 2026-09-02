using static DSAExperimentation.LeetCode.ImplementRouter.ImplementRouterSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementRouter;

// Harness only. Both strategies are ImplementRouterSolution's - this file
// replays LeetCode's published call sequences against each IRouterStrategy
// implementation via a small operation script, so a failure still names the
// strategy that broke even though the "input" here is a sequence of mutating/
// querying calls rather than a single argument tuple. RouterOp.Apply is pure
// dispatch (which method to call with which arguments) - no packet-storage
// logic of its own.
public sealed class ImplementRouterTests
{
    public static TheoryData<int, RouterOp[], object?[]> Examples =>
        new()
        {
            {
                3,
                [
                    RouterOp.AddPacket(1, 4, 90),
                    RouterOp.AddPacket(2, 5, 90),
                    RouterOp.AddPacket(1, 4, 90),
                    RouterOp.AddPacket(3, 5, 95),
                    RouterOp.AddPacket(4, 5, 105),
                    RouterOp.ForwardPacket(),
                    RouterOp.AddPacket(5, 2, 110),
                    RouterOp.GetCount(5, 100, 110),
                ],
                [true, true, false, true, true, new[] { 2, 5, 90 }, true, 1]
            },
            {
                2,
                [
                    RouterOp.AddPacket(7, 4, 90),
                    RouterOp.ForwardPacket(),
                    RouterOp.ForwardPacket(),
                ],
                [true, new[] { 7, 4, 90 }, Array.Empty<int>()]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RouterByLinearScan_LeetCodeExamples_RepliesMatchPublishedOutputs(
        int memoryLimit, RouterOp[] operations, object?[] expected) =>
        RunScript(new RouterByLinearScan(memoryLimit), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RouterByBinarySearchIndex_LeetCodeExamples_RepliesMatchPublishedOutputs(
        int memoryLimit, RouterOp[] operations, object?[] expected) =>
        RunScript(new RouterByBinarySearchIndex(memoryLimit), operations, expected);

    private static void RunScript(IRouterStrategy strategy, RouterOp[] operations, object?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            AssertMatches(expected[i], operations[i].Apply(strategy));
        }
    }

    // int[] results (forwardPacket) need element-wise comparison; bool/int
    // results (addPacket/getCount) compare fine as plain objects.
    private static void AssertMatches(object? expected, object? actual)
    {
        if (expected is int[] expectedPacket)
        {
            Assert.Equal(expectedPacket, Assert.IsType<int[]>(actual));
        }
        else
        {
            Assert.Equal(expected, actual);
        }
    }
}

// One call in a Router script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script
// (like Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct RouterOp
{
    private readonly Kind _kind;
    private readonly int _first;
    private readonly int _second;
    private readonly int _third;

    private RouterOp(Kind kind, int first, int second, int third)
    {
        _kind = kind;
        _first = first;
        _second = second;
        _third = third;
    }

    public static RouterOp AddPacket(int source, int destination, int timestamp) =>
        new(Kind.AddPacket, source, destination, timestamp);

    public static RouterOp ForwardPacket() => new(Kind.ForwardPacket, 0, 0, 0);

    public static RouterOp GetCount(int destination, int startTime, int endTime) =>
        new(Kind.GetCount, destination, startTime, endTime);

    // bool for addPacket, int[] for forwardPacket, int for getCount - boxed
    // uniformly so a script runner can assert against one expected value per
    // operation regardless of which method it dispatches to. Internal, not
    // public: IRouterStrategy is internal to ImplementRouterSolution, and only
    // this same assembly's RunScript ever calls Apply.
    internal object? Apply(IRouterStrategy strategy) => _kind switch
    {
        Kind.AddPacket => strategy.AddPacket(_first, _second, _third),
        Kind.ForwardPacket => strategy.ForwardPacket(),
        _ => strategy.GetCount(_first, _second, _third),
    };

    private enum Kind
    {
        AddPacket,
        ForwardPacket,
        GetCount,
    }
}
