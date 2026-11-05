using DSAExperimentation.Domain.Games.Sudoku.Regions;

namespace DSAExperimentation.Domain.Games.Sudoku;

// The 6x6 Sudoku: boxes two rows tall and three columns wide, so six of them tile a board six a
// side, holding the digits '1' to '6'. The box is not square, which is what tells its rows from its
// columns. The board's side is the contract's, derived from the box. It is written in the classic
// grid's notation, '.' for an empty cell.
/// <summary>6x6 Sudoku: a 6x6 board of boxes two rows tall and three columns wide.</summary>
public readonly struct SixBySixSudoku : ISudokuGrid, IBoxPartition<SixBySixSudoku>
{
    static char ISudokuGrid.EmptyCell => ClassicSudoku.EmptyCell;

    /// <inheritdoc/>
    public static int BoxRows => 2;

    /// <inheritdoc/>
    public static int BoxColumns => 3;
}
