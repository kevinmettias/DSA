using DSAExperimentation.Domain.Games.Sudoku;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku;

// The 6x6 grid is the fact itself: a box two rows tall and three columns wide, and the board's side
// their product (SudokuTests reads it through the rule).
public sealed partial class SixBySixSudokuTests
{
    [Fact]
    public void BoxRows_IsTwo() => Assert.Equal(2, SixBySixSudoku.BoxRows);

    [Fact]
    public void BoxColumns_IsThree() => Assert.Equal(3, SixBySixSudoku.BoxColumns);
}
