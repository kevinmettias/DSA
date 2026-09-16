using DSAExperimentation.LeetCode.JumpGameII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameII;

// Harness only: both strategies live in JumpGameIISolution - the textbook greedy
// two-pointer scan, and modeling reachability as an implicit unweighted-hop graph
// (index i -> every index one jump away) answered with this repo's own
// ShortestPath.Dijkstra.
public sealed partial class JumpGameIITests
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
    public void MinJumpsByDijkstraOverHopGraph_LeetCodeExamples_ReturnsMinimumJumpCount(int[] nums, int expected) =>
        Assert.Equal(expected, JumpGameIISolution.MinJumpsByDijkstraOverHopGraph(nums));
}
