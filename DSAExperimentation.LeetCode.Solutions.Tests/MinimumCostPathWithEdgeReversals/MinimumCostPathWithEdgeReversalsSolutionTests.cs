using DSAExperimentation.LeetCode.Solutions.MinimumCostPathWithEdgeReversals;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumCostPathWithEdgeReversals;

// Harness only. The augmented digraph itself is ReversalGraph and both Dijkstra
// strategies are MinimumCostPathWithEdgeReversalsSolution's. CatalogCaseTests holds them
// to LeetCode's published examples (ARCHITECTURE 17.13); this file adds a destination no
// edge reaches, which no published example poses.
public sealed partial class MinimumCostPathWithEdgeReversalsSolutionTests
{
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
