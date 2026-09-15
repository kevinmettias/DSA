using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.Algorithms.ShortestPaths;

public sealed class ChebyshevHeuristicTests
{
    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(0, 0, 0, 3, 3)]
    [InlineData(0, 0, 2, 3, 3)]
    [InlineData(1, 1, 3, 4, 3)]
    [InlineData(-1, 0, 3, 4, 4)]
    public void Estimate_ReturnsTheLargerAxisDistanceBecauseADiagonalStepCostsOne(
        int row, int col, int targetRow, int targetCol, int expected) =>
        Assert.Equal(
            expected,
            ChebyshevHeuristic.Estimate(new WeightedGridNode(row, col), new WeightedGridNode(targetRow, targetCol)));

    [Fact]
    public void Estimate_NullTarget_ReturnsZero() => Assert.Equal(0, ChebyshevHeuristic.Estimate(new WeightedGridNode(5, 5), null));

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

        Assert.Equal(ChebyshevHeuristic.Estimate(first, second), ChebyshevHeuristic.Estimate(second, first));
    }
}
