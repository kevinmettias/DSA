using DSAExperimentation.LeetCode.MinimumCostWalkInWeightedGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostWalkInWeightedGraph;

// Harness only. Both strategies are MinimumCostWalkInWeightedGraphSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class MinimumCostWalkInWeightedGraphTests
{
    public static TheoryData<int, int[][], int[][], int[]> Examples =>
        new()
        {
            {
                5,
                [[0, 1, 7], [1, 3, 7], [1, 2, 1]],
                [[0, 3], [3, 4]],
                [1, -1]
            },
            {
                3,
                [[0, 2, 7], [0, 1, 15], [1, 2, 6], [1, 2, 1]],
                [[1, 2]],
                [0]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByBruteForceWalk_LeetCodeExamples_ReturnsWalkCosts(
        int n, int[][] edges, int[][] query, int[] expected)
    {
        var actual = MinimumCostWalkInWeightedGraphSolution.MinimumCostByBruteForceWalk(n, edges, query);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByUnionFind_LeetCodeExamples_ReturnsWalkCosts(
        int n, int[][] edges, int[][] query, int[] expected)
    {
        var actual = MinimumCostWalkInWeightedGraphSolution.MinimumCostByUnionFind(n, edges, query);

        Assert.Equal(expected, actual);
    }
}
