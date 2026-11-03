using DSAExperimentation.Domain.Games.Sudoku;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku;

// The classic grid is the fact itself, so the tests state it: the board's side follows from the box's.
public sealed partial class ClassicSudokuTests
{
    [Fact]
    public void BoxSize_IsThree() => Assert.Equal(3, ClassicSudoku.BoxSize);

    [Fact]
    public void Size_IsTheBoxSizeSquared() => Assert.Equal(ClassicSudoku.BoxSize * ClassicSudoku.BoxSize, ClassicSudoku.Size);

    [Fact]
    public void EmptyCell_IsTheDot() => Assert.Equal('.', ClassicSudoku.EmptyCell);
}
