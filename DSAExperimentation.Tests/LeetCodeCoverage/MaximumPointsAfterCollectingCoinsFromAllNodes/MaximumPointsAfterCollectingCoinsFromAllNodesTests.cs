using DSAExperimentation.LeetCode.MaximumPointsAfterCollectingCoinsFromAllNodes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumPointsAfterCollectingCoinsFromAllNodes;

// Harness only: the algorithms live in
// MaximumPointsAfterCollectingCoinsFromAllNodesSolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed class MaximumPointsAfterCollectingCoinsFromAllNodesTests
{
    public static TheoryData<int[][], int[], int, long> Examples =>
        new()
        {
            { [[0, 1], [1, 2], [2, 3]], [10, 10, 3, 3], 5, 11 },
            { [[0, 1], [0, 2]], [8, 4, 4], 0, 16 },
            { [[0, 1]], [5, 5], 100, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPointsByMemoizedRecursion_LeetCodeExamples_ReturnsMaximumPoints(
        int[][] edges, int[] coins, int cost, long expected)
    {
        var actual = MaximumPointsAfterCollectingCoinsFromAllNodesSolution.MaxPointsByMemoizedRecursion(edges, coins, cost);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPointsByTreeFold_LeetCodeExamples_ReturnsMaximumPoints(
        int[][] edges, int[] coins, int cost, long expected)
    {
        var actual = MaximumPointsAfterCollectingCoinsFromAllNodesSolution.MaxPointsByTreeFold(edges, coins, cost);

        Assert.Equal(expected, actual);
    }
}
