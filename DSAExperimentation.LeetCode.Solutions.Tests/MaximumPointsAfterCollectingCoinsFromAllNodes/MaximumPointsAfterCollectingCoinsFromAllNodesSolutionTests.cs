using DSAExperimentation.LeetCode.Solutions.MaximumPointsAfterCollectingCoinsFromAllNodes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumPointsAfterCollectingCoinsFromAllNodes;

// Harness only: the algorithms live in
// MaximumPointsAfterCollectingCoinsFromAllNodesSolution. One test method per
// strategy over one shared set of cases, so a failure names the strategy that
// broke. The one case is a two-node tree whose cost is high enough that halving
// beats collecting at both nodes. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MaximumPointsAfterCollectingCoinsFromAllNodesSolutionTests
{
    public static TheoryData<int[][], int[], int, long> Cases =>
        new()
        {
            { [[0, 1]], [5, 5], 100, 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxPointsByMemoizedRecursion_Cases_ReturnsMaximumPoints(
        int[][] edges, int[] coins, int cost, long expected)
    {
        var actual = MaximumPointsAfterCollectingCoinsFromAllNodesSolution.MaxPointsByMemoizedRecursion(edges, coins, cost);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxPointsByTreeFold_Cases_ReturnsMaximumPoints(
        int[][] edges, int[] coins, int cost, long expected)
    {
        var actual = MaximumPointsAfterCollectingCoinsFromAllNodesSolution.MaxPointsByTreeFold(edges, coins, cost);

        Assert.Equal(expected, actual);
    }
}
