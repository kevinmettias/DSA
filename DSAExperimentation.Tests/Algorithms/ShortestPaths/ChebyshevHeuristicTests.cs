using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

public sealed partial class ChebyshevHeuristicTests
{
    public static TheoryData<EstimateCase> Examples =>
        new()
        {
            { new EstimateCase(Row: 0, Col: 0, TargetRow: 0, TargetCol: 0, Expected: 0) },
            { new EstimateCase(Row: 0, Col: 0, TargetRow: 0, TargetCol: 3, Expected: 3) },
            { new EstimateCase(Row: 0, Col: 0, TargetRow: 2, TargetCol: 3, Expected: 3) },
            { new EstimateCase(Row: 1, Col: 1, TargetRow: 3, TargetCol: 4, Expected: 3) },
            { new EstimateCase(Row: -1, Col: 0, TargetRow: 3, TargetCol: 4, Expected: 4) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void Estimate_ReturnsTheLargerAxisDistanceBecauseADiagonalStepCostsOne(EstimateCase example)
    {
        var distance = ChebyshevHeuristic.Estimate(
            new WeightedGridNode(example.Row, example.Col),
            new WeightedGridNode(example.TargetRow, example.TargetCol));

        Assert.Equal(example.Expected, distance);
    }

    [Fact]
    public void Estimate_NullTarget_ReturnsZero()
    {
        var distance = ChebyshevHeuristic.Estimate(new WeightedGridNode(5, 5), null);

        Assert.Equal(0, distance);
    }

    [Fact]
    public void Estimate_NeverExceedsTheManhattanDistance()
    {
        // King moves can cover both axes at once, so Chebyshev <= Manhattan always.
        var from = new WeightedGridNode(1, 2);
        var to = new WeightedGridNode(7, 5);

        Assert.True(ChebyshevHeuristic.Estimate(from, to) <= ManhattanHeuristic.Estimate(from, to));
    }

    [Fact]
    public void Estimate_IsSymmetricInItsTwoNodes()
    {
        var first = new WeightedGridNode(0, 9);
        var second = new WeightedGridNode(4, 2);
        var forward = ChebyshevHeuristic.Estimate(first, second);
        var backward = ChebyshevHeuristic.Estimate(second, first);

        Assert.Equal(forward, backward);
    }

    // One grid pair: the two endpoints the estimate is taken between, and the distance the
    // larger axis alone accounts for. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct EstimateCase(int Row, int Col, int TargetRow, int TargetCol, int Expected);
}
