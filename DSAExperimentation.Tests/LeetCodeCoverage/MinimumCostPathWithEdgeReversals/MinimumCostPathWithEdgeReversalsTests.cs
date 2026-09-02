using DSAExperimentation.LeetCode.MinimumCostPathWithEdgeReversals;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostPathWithEdgeReversals;

// Harness only. The augmented digraph itself is ReversalGraph and both Dijkstra
// strategies are MinimumCostPathWithEdgeReversalsSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class MinimumCostPathWithEdgeReversalsTests
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
        int n, int[][] edges, int expected) =>
        Assert.Equal(expected, MinimumCostPathWithEdgeReversalsSolution.MinCostByBruteForceDijkstra(n, edges));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByShortestPathDijkstra_LeetCodeExamples_ReturnsCheapestReversalAwarePath(
        int n, int[][] edges, int expected) =>
        Assert.Equal(expected, MinimumCostPathWithEdgeReversalsSolution.MinCostByShortestPathDijkstra(n, edges));

    [Fact]
    public void MinCostByBruteForceDijkstra_DestinationUnreachable_ReturnsNegativeOne() =>
        Assert.Equal(-1, MinimumCostPathWithEdgeReversalsSolution.MinCostByBruteForceDijkstra(3, [[0, 1, 5]]));

    [Fact]
    public void MinCostByShortestPathDijkstra_DestinationUnreachable_ReturnsNegativeOne() =>
        Assert.Equal(-1, MinimumCostPathWithEdgeReversalsSolution.MinCostByShortestPathDijkstra(3, [[0, 1, 5]]));
}
