using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

public sealed partial class ManhattanHeuristicTests
{
    public static TheoryData<EstimateCase> Examples =>
        new()
        {
            { new EstimateCase(Row: 0, Col: 0, TargetRow: 0, TargetCol: 0, Expected: 0) },
            { new EstimateCase(Row: 0, Col: 0, TargetRow: 0, TargetCol: 3, Expected: 3) },
            { new EstimateCase(Row: 0, Col: 0, TargetRow: 2, TargetCol: 3, Expected: 5) },
            { new EstimateCase(Row: 2, Col: 3, TargetRow: 0, TargetCol: 0, Expected: 5) },
            { new EstimateCase(Row: -1, Col: -1, TargetRow: 1, TargetCol: 1, Expected: 4) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void Estimate_ReturnsTheSumOfTheAxisDistances(EstimateCase example)
    {
        var distance = ManhattanHeuristic.Estimate(
            new WeightedGridNode(example.Row, example.Col),
            new WeightedGridNode(example.TargetRow, example.TargetCol));

        Assert.Equal(example.Expected, distance);
    }

    [Fact]
    public void Estimate_NullTarget_ReturnsZeroSoAStarDegradesToDijkstra()
    {
        var distance = ManhattanHeuristic.Estimate(new WeightedGridNode(5, 5), null);

        Assert.Equal(0, distance);
    }

    [Fact]
    public void Estimate_IsSymmetricInItsTwoNodes()
    {
        var first = new WeightedGridNode(1, 4);
        var second = new WeightedGridNode(6, 2);
        var forward = ManhattanHeuristic.Estimate(first, second);
        var backward = ManhattanHeuristic.Estimate(second, first);

        Assert.Equal(forward, backward);
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
        var distance = ManhattanHeuristic.Estimate(target, target);

        Assert.Equal(0, distance);
    }

    // One grid pair: the two endpoints the estimate is taken between, and the sum of the
    // two axis distances. Nested because it is only ever used inside this test class - it
    // is this harness's own vocabulary, not a type another file would import.
    public readonly record struct EstimateCase(int Row, int Col, int TargetRow, int TargetCol, int Expected);
}
