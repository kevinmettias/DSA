using DSAExperimentation.LeetCode.MinimumCostPathWithEdgeReversals;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostPathWithEdgeReversals;

// Harness only. The augmented digraph itself is ReversalGraph and both Dijkstra
// strategies are MinimumCostPathWithEdgeReversalsSolution's - this file just pins
// them to LeetCode's published examples.
public sealed partial class MinimumCostPathWithEdgeReversalsTests
{
    public static TheoryData<int, int[][], int> Examples =>
        new()
        {
            { 4, [[0, 1, 3], [3, 1, 1], [2, 3, 4], [0, 2, 2]], 5 },
            { 4, [[0, 2, 1], [2, 1, 1], [1, 3, 1], [2, 3, 3]], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByBruteForceDijkstra_LeetCodeExamples_ReturnsCheapestReversalAwarePath(
        int nodeCount, int[][] edges, int expected)
    {
        var cost = MinimumCostPathWithEdgeReversalsSolution.MinCostByBruteForceDijkstra(nodeCount, edges);

        Assert.Equal(expected, cost);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByShortestPathDijkstra_LeetCodeExamples_ReturnsCheapestReversalAwarePath(
        int nodeCount, int[][] edges, int expected)
    {
        var cost = MinimumCostPathWithEdgeReversalsSolution.MinCostByShortestPathDijkstra(nodeCount, edges);

        Assert.Equal(expected, cost);
    }

    [Fact]
    public void MinCostByBruteForceDijkstra_DestinationUnreachable_ReturnsNegativeOne()
    {
        var cost = MinimumCostPathWithEdgeReversalsSolution.MinCostByBruteForceDijkstra(3, [[0, 1, 5]]);

        Assert.Equal(-1, cost);
    }

    [Fact]
    public void MinCostByShortestPathDijkstra_DestinationUnreachable_ReturnsNegativeOne()
    {
        var cost = MinimumCostPathWithEdgeReversalsSolution.MinCostByShortestPathDijkstra(3, [[0, 1, 5]]);

        Assert.Equal(-1, cost);
    }
}
