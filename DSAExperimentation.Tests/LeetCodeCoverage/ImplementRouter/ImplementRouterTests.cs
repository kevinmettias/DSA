using DSAExperimentation.LeetCode.ImplementRouter;

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
        RunScript(new ImplementRouterSolution.RouterByLinearScan(memoryLimit), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void RouterByBinarySearchIndex_LeetCodeExamples_RepliesMatchPublishedOutputs(
        int memoryLimit, RouterOp[] operations, object?[] expected) =>
        RunScript(new ImplementRouterSolution.RouterByBinarySearchIndex(memoryLimit), operations, expected);

    private static void RunScript(
        ImplementRouterSolution.IRouterStrategy strategy, RouterOp[] operations, object?[] expected)
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
