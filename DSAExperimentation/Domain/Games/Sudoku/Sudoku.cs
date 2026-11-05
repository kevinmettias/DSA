namespace DSAExperimentation.Domain.Games.Sudoku;

// Sudoku's rule on any grid of box size n, as LC 36 and LC 37 state it for n = 3: no symbol repeats
// in a row, a column or a box. These are the problems' *what* (ARCHITECTURE §17.6). Which cell a
// solver fills next, the order it tries symbols in and how a validator tracks what it has seen are
// each strategy's *how*, and stay with the solutions; the baselines keep their own copies of the
// rules (§17.5). The grid's numbers are static abstract members of a struct, so they fold to
// constants in each caller.
/// <summary>The Sudoku board's size, and the rule a placed symbol obeys, for a grid of any box size.</summary>
/// <typeparam name="TGrid">The grid, such as <see cref="ClassicSudoku"/>.</typeparam>
public static class Sudoku<TGrid>
    where TGrid : struct, ISudokuGrid
{
    /// <summary>The number of rows, of columns and of boxes on the board: the box size squared.</summary>
    public static int Size => TGrid.BoxSize * TGrid.BoxSize;

    /// <summary>Numbers the box a cell lies in, row by row from 0 at the top left.</summary>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <returns>The number of the box holding the cell.</returns>
    public static int BoxOf(int row, int col) => (row / TGrid.BoxSize * TGrid.BoxSize) + (col / TGrid.BoxSize);

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
        var boxRow = row / TGrid.BoxSize * TGrid.BoxSize;
        var boxCol = col / TGrid.BoxSize * TGrid.BoxSize;

        for (var r = boxRow; r < boxRow + TGrid.BoxSize; r++)
        {
            for (var c = boxCol; c < boxCol + TGrid.BoxSize; c++)
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
    /// <returns><see langword="true"/> when no cell is the grid's empty-cell mark.</returns>
    public static bool IsComplete(char[][] board)
    {
        foreach (var cells in board)
        {
            if (Array.IndexOf(cells, TGrid.EmptyCell) >= 0)
            {
                return false;
            }
        }

        return true;
    }
}


// TODO: CONSIDER ADDING GENERIC METHODS FOR SOLVING SUDOKU, VALIDATING BOARDS, OR GENERATING PUZZLES, SIMILAR TO HOW OTHER DOMAINS HAVE GENERIC IMPLEMENTATIONS.

// TODO: CONSIDER GENERALIZING TO NON-SQUARE REGIONS OR DIFFERENT SUDOKU VARIANTS, SUCH AS 6x6 WITH 2x3 BOXES OR 16x16 WITH 4x4 BOXES. OR EVEN IRREGULAR SHAPES LIKE JIGSAW SUDOKU.