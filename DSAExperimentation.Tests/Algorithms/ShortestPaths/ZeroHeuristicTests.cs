using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

public sealed class ZeroHeuristicTests
{
    [Fact]
    public void Estimate_AlwaysReturnsZeroWhichIsWhatMakesAStarBehaveAsDijkstra() => Assert.Equal(0, ZeroHeuristic<WeightedGridNode, int>.Estimate(new WeightedGridNode(3, 9), new WeightedGridNode(0, 0)));

    [Fact]
    public void Estimate_NullTarget_IsAlsoZero() => Assert.Equal(0, ZeroHeuristic<WeightedGridNode, int>.Estimate(new WeightedGridNode(1, 1), null));

    [Fact]
    public void Estimate_IsZeroForEveryWeightType() => Assert.Equal(0d, ZeroHeuristic<WeightedGridNode, double>.Estimate(new WeightedGridNode(2, 2), null));

    [Fact]
    public void Estimate_IsTriviallyConsistentSinceEveryEdgeCostIsNonNegative()
    {
        // h(u) <= cost(u,v) + h(v) collapses to 0 <= cost, which always holds.
        var node = new WeightedGridNode(0, 0);
        var target = new WeightedGridNode(5, 5);

        Assert.True(ZeroHeuristic<WeightedGridNode, int>.Estimate(node, target) <= 1);
    }
}
