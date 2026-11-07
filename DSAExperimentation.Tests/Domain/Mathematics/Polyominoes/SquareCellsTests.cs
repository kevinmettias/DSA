using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes;

// A polyomino's squares are joined by the four orthogonal steps and listed in reading order, an
// order every translation keeps.
public sealed partial class SquareCellsTests
{
    [Fact]
    public void JoiningSteps_AreTheOrthogonalSteps() =>
        Assert.Equal(GridDirections.Orthogonal, SquareCells.JoiningSteps);

    [Fact]
    public void Compare_EarlierRow_ComesFirstWhateverTheColumn() =>
        Assert.True(SquareCells.Compare(new(0, 9), new(1, -9)) < 0);

    [Fact]
    public void Compare_SameRow_ComparesColumns() =>
        Assert.True(SquareCells.Compare(new(2, 3), new(2, 1)) > 0);

    [Fact]
    public void Compare_OneSquare_IsZero() =>
        Assert.Equal(0, SquareCells.Compare(new(2, 3), new(2, 3)));

    [Fact]
    public void Compare_TranslatedPair_KeepsItsOrder() =>
        Assert.Equal(
            Math.Sign(SquareCells.Compare(new(0, 2), new(1, 0))),
            Math.Sign(
                SquareCells.Compare(
                    new GridStep(0, 2) + GridStep.UpRight, new GridStep(1, 0) + GridStep.UpRight)));

    [Fact]
    public void Rectangle_TwoByThree_ListsItsSquaresRowByRow() =>
        Assert.Equal(
            [new(0, 0), new(0, 1), new(0, 2), new(1, 0), new(1, 1), new(1, 2)],
            SquareCells.Rectangle(2, 3).ToArray());

    [Fact]
    public void Rectangle_NoRows_IsEmpty() => Assert.Empty(SquareCells.Rectangle(0, 5));

    [Fact]
    public void Rectangle_NegativeSide_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SquareCells.Rectangle(2, -1));
}
