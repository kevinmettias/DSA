using DSAExperimentation.LeetCode.JumpGameIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameIV;

// Harness only: both strategies live in JumpGameIVSolution - the textbook
// level-order BFS with same-value group pruning, and modeling the same
// i+1/i-1/same-value reachability as an implicit unweighted-hop graph answered
// with this repo's own ShortestPath.Dijkstra.
public sealed class JumpGameIVTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [100, -23, -23, 404, 100, 23, 23, 23, 3, 404], 3 },
            { [7], 0 },
            { [7, 6, 9, 6, 9, 6, 9, 7], 1 },
            { [6, 1, 9], 2 },
            { [1, 2, 3, 4, 5], 4 },
            { [11, 22, 7, 7, 7, 7, 7, 7, 7, 22, 13], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinJumpsByBfsWithGroupPruning_LeetCodeExamples_ReturnsMinimumStepCount(int[] arr, int expected) =>
        Assert.Equal(expected, JumpGameIVSolution.MinJumpsByBfsWithGroupPruning(arr));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinJumpsByDijkstraOverHopGraph_LeetCodeExamples_ReturnsMinimumStepCount(int[] arr, int expected) =>
        Assert.Equal(expected, JumpGameIVSolution.MinJumpsByDijkstraOverHopGraph(arr));
}
