using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

public sealed class ManhattanHeuristicTests
{
    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(0, 0, 0, 3, 3)]
    [InlineData(0, 0, 2, 3, 5)]
    [InlineData(2, 3, 0, 0, 5)]
    [InlineData(-1, -1, 1, 1, 4)]
    public void Estimate_ReturnsTheSumOfTheAxisDistances(
        int row, int col, int targetRow, int targetCol, int expected) =>
        Assert.Equal(
            expected,
            ManhattanHeuristic.Estimate(new WeightedGridNode(row, col), new WeightedGridNode(targetRow, targetCol)));

    [Fact]
    public void Estimate_NullTarget_ReturnsZeroSoAStarDegradesToDijkstra() => Assert.Equal(0, ManhattanHeuristic.Estimate(new WeightedGridNode(5, 5), null));

    [Fact]
    public void Estimate_IsSymmetricInItsTwoNodes()
    {
        var first = new WeightedGridNode(1, 4);
        var second = new WeightedGridNode(6, 2);

        Assert.Equal(ManhattanHeuristic.Estimate(first, second), ManhattanHeuristic.Estimate(second, first));
    }

    [Fact]
    public void Estimate_IsConsistentAcrossEveryUnitStepOfAGrid()
    {
        // Consistency - h(u) <= cost(u,v) + h(v) with cost 1 - is the property
        // ShortestPath.AStar relies on to settle each node exactly once.
        var nodes = WeightedGrid.Build(4, 4);
        var target = nodes[(3, 3)];

        foreach (var node in nodes.Values)
        {
            var here = ManhattanHeuristic.Estimate(node, target);

            foreach (var (weight, neighbour) in node.Edges)
            {
                Assert.True(here <= weight + ManhattanHeuristic.Estimate(neighbour, target));
            }
        }
    }

    [Fact]
    public void Estimate_TargetItself_IsZero()
    {
        var target = new WeightedGridNode(2, 7);

        Assert.Equal(0, ManhattanHeuristic.Estimate(target, target));
    }
}
