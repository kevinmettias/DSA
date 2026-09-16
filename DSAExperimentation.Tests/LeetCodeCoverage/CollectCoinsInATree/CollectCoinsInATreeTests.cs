using DSAExperimentation.LeetCode.CollectCoinsInATree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CollectCoinsInATree;

// Harness only: the coin-aware leaf trim and both of its frontier strategies live in
// CollectCoinsInATreeSolution. One test method per strategy over one shared set of
// examples, so a failure names the strategy that broke.
public sealed partial class CollectCoinsInATreeTests
{
    public static TheoryData<int[][], int[], int> Examples =>
        new()
        {
            { [[0, 1], [1, 2], [2, 3], [3, 4], [4, 5]], [1, 0, 0, 0, 0, 1], 2 },
            { [[0, 1], [0, 2], [1, 3], [1, 4], [2, 5], [5, 6], [5, 7]], [0, 0, 0, 1, 1, 0, 0, 1], 2 },

            // A lone vertex, with and without a coin: nowhere to walk either way.
            { [], [1], 0 },
            { [], [0], 0 },

            // Two coins one hop apart are both inside the radius-2 allowance, so the
            // whole tree is peeled away and no edge is ever walked.
            { [[0, 1]], [1, 1], 0 },

            // The first example's path with one more vertex: three nodes survive the
            // two free layers instead of two, so two edges are walked out and back.
            { [[0, 1], [1, 2], [2, 3], [3, 4], [4, 5], [5, 6]], [1, 0, 0, 0, 0, 0, 1], 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinEdgesByRescan_LeetCodeExamples_ReturnsRoundTripEdgeCount(
        int[][] edges, int[] coins, int expected)
    {
        var actual = CollectCoinsInATreeSolution.MinEdgesByRescan(edges, coins);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinEdgesByLeafQueue_LeetCodeExamples_ReturnsRoundTripEdgeCount(
        int[][] edges, int[] coins, int expected)
    {
        var actual = CollectCoinsInATreeSolution.MinEdgesByLeafQueue(edges, coins);

        Assert.Equal(expected, actual);
    }
}
