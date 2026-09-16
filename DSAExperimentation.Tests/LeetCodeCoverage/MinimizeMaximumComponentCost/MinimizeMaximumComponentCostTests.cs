using DSAExperimentation.LeetCode.MinimizeMaximumComponentCost;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimizeMaximumComponentCost;

// Harness only. The reverse-Kruskal merge itself is
// MinimizeMaximumComponentCostSolution's - this file just pins both strategies
// to LeetCode's published examples.
public sealed class MinimizeMaximumComponentCostTests
{
    public static TheoryData<int, int[][], int, int> Examples =>
        new()
        {
            { 5, [[0, 1, 4], [1, 2, 3], [1, 3, 2], [3, 4, 6]], 2, 4 },
            { 4, [[0, 1, 5], [1, 2, 5], [2, 3, 5]], 1, 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByUnionFind_LeetCodeExamples_ReturnsMinimizedMaximumComponentCost(
        int nodeCount, int[][] edges, int maxComponents, int expected)
    {
        var actual = MinimizeMaximumComponentCostSolution.MinCostByUnionFind(nodeCount, edges, maxComponents);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByKruskalMst_LeetCodeExamples_ReturnsMinimizedMaximumComponentCost(
        int nodeCount, int[][] edges, int maxComponents, int expected)
    {
        var actual = MinimizeMaximumComponentCostSolution.MinCostByKruskalMst(nodeCount, edges, maxComponents);

        Assert.Equal(expected, actual);
    }
}
