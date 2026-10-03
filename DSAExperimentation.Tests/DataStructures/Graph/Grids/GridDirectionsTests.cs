using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Tests.DataStructures.Graph.Grids;

// Order is part of GridDirections' contract - callers whose answers list cells in visit order rely
// on it - so each table's exact sequence is pinned, and each is also checked against the geometry
// that defines it, independently of the literal.
public sealed partial class GridDirectionsTests
{
    // Each table is compared as an array: ImmutableArray's own Equals compares the underlying array
    // reference, not the elements.
    [Fact]
    public void Orthogonal_IsUpDownLeftRight() =>
        Assert.Equal([(-1, 0), (1, 0), (0, -1), (0, 1)], GridDirections.Orthogonal.ToArray());

    [Fact]
    public void Orthogonal_EveryOffset_MovesOneStepAlongOneAxis() =>
        Assert.All(GridDirections.Orthogonal, offset => Assert.Equal(1, Math.Abs(offset.DeltaRow) + Math.Abs(offset.DeltaCol)));

    [Fact]
    public void Diagonal_IsRowMajor() =>
        Assert.Equal([(-1, -1), (-1, 1), (1, -1), (1, 1)], GridDirections.Diagonal.ToArray());

    [Fact]
    public void Diagonal_EveryOffset_MovesOneStepAlongBothAxes() =>
        Assert.All(GridDirections.Diagonal, offset => Assert.True(Math.Abs(offset.DeltaRow) == 1 && Math.Abs(offset.DeltaCol) == 1));

    [Fact]
    public void King_IsTheEightNeighboursInRowMajorOrder() =>
        Assert.Equal(
            [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)],
            GridDirections.King.ToArray());

    [Fact]
    public void King_IsExactlyOrthogonalAndDiagonalTogether() =>
        Assert.Equal(
            GridDirections.Orthogonal.Concat(GridDirections.Diagonal).Order(),
            GridDirections.King.Order());

    [Fact]
    public void Knight_IsRowMajor() =>
        Assert.Equal([(-2, -1), (-2, 1), (-1, -2), (-1, 2), (1, -2), (1, 2), (2, -1), (2, 1)], GridDirections.Knight.ToArray());

    [Fact]
    public void Knight_EveryOffset_IsTwoStepsOneWayAndOneTheOther() =>
        Assert.All(GridDirections.Knight, offset => Assert.Equal(
            (1, 2),
            (Math.Min(Math.Abs(offset.DeltaRow), Math.Abs(offset.DeltaCol)), Math.Max(Math.Abs(offset.DeltaRow), Math.Abs(offset.DeltaCol)))));
}
