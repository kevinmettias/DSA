using DSAExperimentation.Domain.Games.Sudoku.Regions;

namespace DSAExperimentation.Domain.Games.Sudoku;

// The 16x16 Sudoku: boxes of 4x4, so a board sixteen a side holding sixteen symbols. The box is
// square, so one side states it, and the board's side is the contract's, derived from the box.
// Puzzles write the sixteen symbols in no one agreed way (0 to 9 then A to F, 1 to 9 then A to G,
// or A to P), and the rule only compares them, so no alphabet is fixed here. It is written in the
// classic grid's notation, '.' for an empty cell.
/// <summary>16x16 Sudoku: a 16x16 board of 4x4 boxes.</summary>
public readonly struct SixteenBySixteenSudoku : ISudokuGrid, IBoxPartition<SixteenBySixteenSudoku>
{
    private const int BoxSide = 4;

    static char ISudokuGrid.EmptyCell => ClassicSudoku.EmptyCell;

    /// <inheritdoc/>
    public static int BoxRows => BoxSide;

    /// <inheritdoc/>
    public static int BoxColumns => BoxSide;
}
