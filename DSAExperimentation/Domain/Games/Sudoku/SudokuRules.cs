namespace DSAExperimentation.Domain.Games.Sudoku;

// Sudoku's world as LC 36 and LC 37 both state it: a 9x9 grid made of nine 3x3 boxes, holding the
// digits '1' to '9', with '.' marking an empty cell, where no digit may repeat in a row, a column or
// a box. These are the problems' *what* (ARCHITECTURE §17.6). Which cell a solver fills next, the
// order it tries digits in and how a validator tracks what it has seen are each strategy's *how*, and
// stay with the solutions; the baselines keep their own copies of the rules (§17.5).
/// <summary>The Sudoku board's dimensions, its notation, and the rule a placed digit obeys.</summary>
public static class SudokuRules
{
    /// <summary>The number of rows, of columns and of boxes on the board.</summary>
    public const int Size = 9;

    /// <summary>The side of one box, in cells.</summary>
    public const int BoxSize = 3;

    /// <summary>The character that marks an empty cell.</summary>
    public const char EmptyCell = '.';

    /// <summary>The smallest digit a cell can hold.</summary>
    public const char LowestDigit = '1';

    /// <summary>The largest digit a cell can hold.</summary>
    public const char HighestDigit = '9';

    /// <summary>Numbers the box a cell lies in, row by row from 0 at the top left to 8 at the bottom right.</summary>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <returns>The number of the box holding the cell.</returns>
    public static int BoxOf(int row, int col) => (row / BoxSize * BoxSize) + (col / BoxSize);

    /// <summary>Whether a digit may go in an empty cell: it appears nowhere in the cell's row, column or box.</summary>
    /// <param name="board">The board, one array of <see cref="Size"/> characters per row.</param>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <param name="digit">The digit to place.</param>
    /// <returns><see langword="true"/> when no unit of the cell holds the digit already.</returns>
    public static bool CanPlace(char[][] board, int row, int col, char digit) =>
        !IsInRowOrColumn(board, row, col, digit) && !IsInBox(board, row, col, digit);

    // Whether the cell's row or column already holds the digit.
    private static bool IsInRowOrColumn(char[][] board, int row, int col, char digit)
    {
        for (var i = 0; i < Size; i++)
        {
            if (board[row][i] == digit || board[i][col] == digit)
            {
                return true;
            }
        }

        return false;
    }

    // Whether the 3x3 box around the cell already holds the digit.
    private static bool IsInBox(char[][] board, int row, int col, char digit)
    {
        var boxRow = row / BoxSize * BoxSize;
        var boxCol = col / BoxSize * BoxSize;

        for (var r = boxRow; r < boxRow + BoxSize; r++)
        {
            for (var c = boxCol; c < boxCol + BoxSize; c++)
            {
                if (board[r][c] == digit)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>Whether every cell of the board holds a digit.</summary>
    /// <param name="board">The board, one array of <see cref="Size"/> characters per row.</param>
    /// <returns><see langword="true"/> when no cell is <see cref="EmptyCell"/>.</returns>
    public static bool IsComplete(char[][] board)
    {
        foreach (var cells in board)
        {
            if (Array.IndexOf(cells, EmptyCell) >= 0)
            {
                return false;
            }
        }

        return true;
    }
}
