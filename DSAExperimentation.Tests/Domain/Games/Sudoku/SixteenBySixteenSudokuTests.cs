using DSAExperimentation.Domain.Games.Sudoku;

namespace DSAExperimentation.Tests.Domain.Games.Sudoku;

// The 16x16 grid is the fact itself: a square box four a side, and the board's side its square
// (SudokuTests reads it through the rule).
public sealed partial class SixteenBySixteenSudokuTests
{
    [Fact]
    public void BoxRows_IsFour() => Assert.Equal(4, SixteenBySixteenSudoku.BoxRows);

    [Fact]
    public void BoxColumns_IsFour() => Assert.Equal(4, SixteenBySixteenSudoku.BoxColumns);
}
