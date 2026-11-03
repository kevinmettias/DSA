namespace DSAExperimentation.Domain.Games.Sudoku;

// Classic Sudoku as LC 36 and LC 37 both state it: boxes of 3x3, so a 9x9 board holding the digits
// '1' to '9' (SudokuDigits), with '.' marking an empty cell. The board's side is derived from the
// box's, not stated beside it. Both stay constants for a caller that needs one; the grid contract
// reads them.
/// <summary>Classic Sudoku: a 9x9 board of 3x3 boxes, with '.' for an empty cell.</summary>
public readonly struct ClassicSudoku : ISudokuGrid
{
    /// <summary>The side of a box, 3.</summary>
    public const int BoxSize = 3;

    /// <summary>The side of the board, and the number of digits: the box size squared, 9.</summary>
    public const int Size = BoxSize * BoxSize;

    /// <summary>The mark an empty cell is written with, '.'.</summary>
    public const char EmptyCell = '.';

    static int ISudokuGrid.BoxSize => BoxSize;

    static char ISudokuGrid.EmptyCell => EmptyCell;
}
