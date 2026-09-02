using DSAExperimentation.LeetCode.MinimumCostToBuyApplesII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToBuyApplesII;

// Harness only: both strategies live in MinimumCostToBuyApplesIISolution. One test
// method per strategy over one shared set of LeetCode's own examples, so a failure
// names the strategy that broke.
public sealed class MinimumCostToBuyApplesIITests
{
    public static TheoryData<int, int[], int[][], long[]> Examples =>
        new()
        {
            { 2, [8, 3], [[0, 1, 1, 2]], [6, 3] },
            { 3, [9, 4, 6], [[0, 1, 1, 3], [1, 2, 4, 2]], [8, 4, 6] },
            { 3, [10, 11, 1], [[0, 2, 1, 3], [1, 2, 3, 4], [0, 1, 5, 2]], [5, 11, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostsByBruteForceDijkstra_LeetCodeExamples_ReturnsMinimumCostPerShop(
        int n, int[] prices, int[][] roads, long[] expected)
    {
        var actual = MinimumCostToBuyApplesIISolution.MinCostsByBruteForceDijkstra(n, prices, roads);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostsByReduceGraph_LeetCodeExamples_ReturnsMinimumCostPerShop(
        int n, int[] prices, int[][] roads, long[] expected)
    {
        var actual = MinimumCostToBuyApplesIISolution.MinCostsByReduceGraph(n, prices, roads);

        Assert.Equal(expected, actual);
    }
}
