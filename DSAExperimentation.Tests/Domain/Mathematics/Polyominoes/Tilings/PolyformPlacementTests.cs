using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Tilings;

// A placement carries its piece, the orientation it is put down in and where its first cell lands.
public sealed partial class PolyformPlacementTests
{
    private static readonly PolyformPlacement<GridStep, SquareCells> TOnTheThirdRow =
        new(2, Tetrominoes.T, new(3, 1));

    [Fact]
    public void Piece_IsThePiecesIndex() => Assert.Equal(2, TOnTheThirdRow.Piece);

    [Fact]
    public void Orientation_IsTheOrientationPutDown() =>
        Assert.Equal(Tetrominoes.T, TOnTheThirdRow.Orientation);

    [Fact]
    public void Anchor_IsWhereTheFirstCellLands() =>
        Assert.Equal(new GridStep(3, 1), TOnTheThirdRow.Anchor);
}
