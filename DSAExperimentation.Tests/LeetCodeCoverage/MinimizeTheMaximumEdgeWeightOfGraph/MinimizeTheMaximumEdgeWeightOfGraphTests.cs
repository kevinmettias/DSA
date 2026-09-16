using DSAExperimentation.LeetCode.MinimizeTheMaximumEdgeWeightOfGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimizeTheMaximumEdgeWeightOfGraph;

// Harness only. The reversed, weight-filtered graph is
// MinimizeTheMaximumEdgeWeightOfGraph's own EdgeWeightGraph/EdgeWeightTopology and
// both feasibility searches are MinimizeTheMaximumEdgeWeightOfGraphSolution's -
// this file just pins them to LeetCode's published examples, including the two
// unreachable cases where even keeping every edge cannot reach every node.
public sealed class MinimizeTheMaximumEdgeWeightOfGraphTests
{
    public static TheoryData<int, int[][], int, int> Examples =>
        new()
        {
            { 5, [[1, 0, 1], [2, 0, 2], [3, 0, 1], [4, 3, 1], [2, 1, 1]], 2, 1 },
            { 5, [[0, 1, 1], [0, 2, 2], [0, 3, 1], [0, 4, 1], [1, 2, 1], [1, 4, 1]], 1, -1 },
            { 5, [[1, 2, 1], [1, 3, 3], [1, 4, 5], [2, 3, 2], [3, 4, 2], [4, 0, 1]], 1, 2 },
            { 5, [[1, 2, 1], [1, 3, 3], [1, 4, 5], [2, 3, 2], [4, 0, 1]], 1, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMaxWeightByBinarySearchBfs_LeetCodeExamples_ReturnsSmallestFeasibleMaxWeight(
        int nodeCount, int[][] edges, int threshold, int expected)
    {
        var actual =
            MinimizeTheMaximumEdgeWeightOfGraphSolution.MinMaxWeightByBinarySearchBfs(nodeCount, edges, threshold);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinMaxWeightByReduceGraphBinarySearch_LeetCodeExamples_ReturnsSmallestFeasibleMaxWeight(
        int nodeCount, int[][] edges, int threshold, int expected)
    {
        var actual =
            MinimizeTheMaximumEdgeWeightOfGraphSolution.MinMaxWeightByReduceGraphBinarySearch(nodeCount, edges, threshold);

        Assert.Equal(expected, actual);
    }
}
