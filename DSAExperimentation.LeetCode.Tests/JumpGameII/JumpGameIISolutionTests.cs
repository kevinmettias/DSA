using DSAExperimentation.LeetCode.JumpGameII;

namespace DSAExperimentation.LeetCode.Tests.JumpGameII;

// Harness only: both strategies live in JumpGameIISolution - the textbook greedy
// two-pointer scan, and modeling reachability as an implicit unweighted-hop graph
// (index i -> every index one jump away) answered with this repo's own
// ShortestPath.Dijkstra. That hop graph is asserted on its own, index by index.
public sealed partial class JumpGameIISolutionTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 3, 1, 1, 4], 2 },
            { [2, 3, 0, 1, 4], 2 },
            { [0], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinJumpsByGreedyTwoPointer_LeetCodeExamples_ReturnsMinimumJumpCount(int[] nums, int expected) =>
        Assert.Equal(expected, JumpGameIISolution.MinJumpsByGreedyTwoPointer(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinJumpsByBreadthFirstDistances_LeetCodeExamples_ReturnsMinimumJumpCount(int[] nums, int expected) =>
        Assert.Equal(expected, JumpGameIISolution.MinJumpsByBreadthFirstDistances(nums));

    // LeetCode's second example, 2 3 0 1 4: index i reaches i + 1 through i + nums[i],
    // clamped to the last index. 0 -> 1, 2; 1 -> 2, 3, 4; 2 cannot
    // jump; 3 -> 4; and 4's reach of 8 is clamped to itself, so it has no hop at all.
    [Fact]
    public void BuildHopGraph_LeetCodeSecondExample_WiresAUnitHopToEveryIndexOneJumpReaches()
    {
        var nodes = JumpGameIISolution.BuildHopGraph([2, 3, 0, 1, 4]);
        var indices = nodes.Select(node => node.Index);
        var targets = nodes.Select(node => node.Targets.Select(target => target.Index).ToArray());

        Assert.Equal([0, 1, 2, 3, 4], indices);
        Assert.Equal([[1, 2], [2, 3, 4], [], [4], []], targets);
    }
}
