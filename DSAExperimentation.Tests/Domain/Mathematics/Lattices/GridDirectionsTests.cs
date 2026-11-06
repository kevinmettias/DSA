using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Metrics;

namespace DSAExperimentation.Tests.Domain.Mathematics.Lattices;

// Order is part of GridDirections' contract - callers whose answers list cells in visit order rely
// on it - so each table's exact sequence is pinned, and each is also checked against the geometry
// that defines it, independently of the literal.
public sealed partial class GridDirectionsTests
{
    // Each table is compared as an array of (row, column) pairs: ImmutableArray's own Equals compares
    // the underlying array reference, not the elements, and the pairs state each named step's value.
    [Fact]
    public void Orthogonal_IsUpDownLeftRight() =>
        Assert.Equal([(-1, 0), (1, 0), (0, -1), (0, 1)], GridDirections.Orthogonal.Select(Pair).ToArray());

    [Fact]
    public void Orthogonal_EveryOffset_MovesOneStepAlongOneAxis() =>
        Assert.All(GridDirections.Orthogonal, offset => Assert.Equal(1, Math.Abs(offset.DeltaRow) + Math.Abs(offset.DeltaCol)));

    [Fact]
    public void Diagonal_IsRowMajor() =>
        Assert.Equal([(-1, -1), (-1, 1), (1, -1), (1, 1)], GridDirections.Diagonal.Select(Pair).ToArray());

    [Fact]
    public void Diagonal_EveryOffset_MovesOneStepAlongBothAxes() =>
        Assert.All(GridDirections.Diagonal, offset => Assert.True(Math.Abs(offset.DeltaRow) == 1 && Math.Abs(offset.DeltaCol) == 1));

    [Fact]
    public void Octilinear_IsTheEightNeighboursInRowMajorOrder() =>
        Assert.Equal(
            [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)],
            GridDirections.Octilinear.Select(Pair).ToArray());

    [Fact]
    public void Octilinear_IsExactlyOrthogonalAndDiagonalTogether() =>
        Assert.Equal(
            GridDirections.Orthogonal.Concat(GridDirections.Diagonal).Select(Pair).Order(),
            GridDirections.Octilinear.Select(Pair).Order());

    [Fact]
    public void LShaped_IsRowMajor() =>
        Assert.Equal([(-2, -1), (-2, 1), (-1, -2), (-1, 2), (1, -2), (1, 2), (2, -1), (2, 1)], GridDirections.LShaped.Select(Pair).ToArray());

    [Fact]
    public void LShaped_EveryOffset_IsTwoStepsOneWayAndOneTheOther() =>
        Assert.All(GridDirections.LShaped, offset => Assert.Equal(
            (1, 2),
            (Math.Min(Math.Abs(offset.DeltaRow), Math.Abs(offset.DeltaCol)), Math.Max(Math.Abs(offset.DeltaRow), Math.Abs(offset.DeltaCol)))));

    [Fact]
    public void Clockwise_IsUpRightDownLeft() =>
        Assert.Equal([(-1, 0), (0, 1), (1, 0), (0, -1)], GridDirections.Clockwise.Select(Pair).ToArray());

    [Fact]
    public void Clockwise_EachStep_IsTheOneBeforeItTurnedAQuarter() =>
        Assert.All(Enumerable.Range(1, GridDirections.Clockwise.Length - 1), index => Assert.Equal(
            GridStep.Rotate(GridDirections.Clockwise[index - 1]),
            GridDirections.Clockwise[index]));

    [Fact]
    public void LineDirections_IsRowColumnDiagonalAntiDiagonal() =>
        Assert.Equal(
            [(0, 1), (1, 0), (1, 1), (1, -1)],
            GridDirections.LineDirections.Select(Pair).ToArray());

    [Fact]
    public void LineDirections_WithTheirOpposites_AreOctilinear() =>
        Assert.Equal(
            GridDirections.Octilinear.Select(Pair).Order(),
            GridDirections.LineDirections.SelectMany(BothWays).Select(Pair).Order());

    [Fact]
    public void TurnRight_EachIndex_IndexesTheStepTurnedClockwise() =>
        Assert.All(ClockwiseIndexes(), index => Assert.Equal(
            GridStep.Rotate(GridDirections.Clockwise[index]),
            GridDirections.Clockwise[GridDirections.TurnRight(index)]));

    [Fact]
    public void TurnLeft_EachIndex_IndexesTheStepTurnedAnticlockwise() =>
        Assert.All(ClockwiseIndexes(), index => Assert.Equal(
            LatticeSymmetry.Turn(GridDirections.Clockwise[index], -1),
            GridDirections.Clockwise[GridDirections.TurnLeft(index)]));

    [Fact]
    public void OppositeOrthogonal_EachIndex_IndexesTheReversedStep() =>
        Assert.All(Enumerable.Range(0, GridDirections.Orthogonal.Length), index => Assert.Equal(
            -GridDirections.Orthogonal[index],
            GridDirections.Orthogonal[GridDirections.OppositeOrthogonal(index)]));

    [Fact]
    public void WithinManhattan_RangeOne_IsTheOrthogonalStepsRowByRow() =>
        Assert.Equal([GridStep.Up, GridStep.Left, GridStep.Right, GridStep.Down], GridDirections.WithinManhattan(1).ToArray());

    [Fact]
    public void WithinChebyshev_RangeOne_IsOctilinear() =>
        Assert.Equal(GridDirections.Octilinear.ToArray(), GridDirections.WithinChebyshev(1).ToArray());

    [Fact]
    public void WithinManhattan_RangeZero_IsEmpty() => Assert.Empty(GridDirections.WithinManhattan(0));

    [Theory]
    [MemberData(nameof(Ranges))]
    public void WithinManhattan_EachRange_HoldsTheDiamondLessTheCell(int range)
    {
        var steps = GridDirections.WithinManhattan(range);

        Assert.Equal(2 * range * (range + 1), steps.Length);
        Assert.All(steps, step =>
            Assert.InRange(L1Norm.Length(step.DeltaRow, step.DeltaCol), 1, range));
    }

    [Theory]
    [MemberData(nameof(Ranges))]
    public void WithinChebyshev_EachRange_HoldsTheSquareLessTheCell(int range)
    {
        var steps = GridDirections.WithinChebyshev(range);

        Assert.Equal(((2 * range) + 1) * ((2 * range) + 1) - 1, steps.Length);
        Assert.All(steps, step =>
            Assert.InRange(LInfinityNorm.Length(step.DeltaRow, step.DeltaCol), 1, range));
    }

    // A neighbourhood agrees with the lattice when every symmetry maps it onto itself.
    [Theory]
    [MemberData(nameof(Ranges))]
    public void WithinManhattan_EachRange_IsKeptByTheSquaresSymmetries(int range) =>
        AssertKeptBySymmetries(GridDirections.WithinManhattan(range));

    [Theory]
    [MemberData(nameof(Ranges))]
    public void WithinChebyshev_EachRange_IsKeptByTheSquaresSymmetries(int range) =>
        AssertKeptBySymmetries(GridDirections.WithinChebyshev(range));

    public static TheoryData<int> Ranges => [1, 2, 3, 5];

    private static GridStep[] BothWays(GridStep step) => [step, -step];

    private static IEnumerable<int> ClockwiseIndexes() => Enumerable.Range(0, GridDirections.Clockwise.Length);

    private static void AssertKeptBySymmetries(IReadOnlyCollection<GridStep> steps) =>
        Assert.All(steps, step =>
        {
            Assert.Contains(GridStep.Rotate(step), steps);
            Assert.Contains(GridStep.Reflect(step), steps);
        });

    private static (int DeltaRow, int DeltaCol) Pair(GridStep step) => (step.DeltaRow, step.DeltaCol);
}
