using DSAExperimentation.Domain.Mathematics.Combinatorics.LatinSquares;

namespace DSAExperimentation.Domain.Games.Sudoku;

// Sudoku's rule on any grid, as LC 36 and LC 37 state it for the classic one: no symbol repeats in
// a row, a column or a region. That is the Latin square's rule (LatinSquare) plus the grid's
// regions (IRegionPartition), and a solved board is a Latin square whose every region holds each
// symbol once. These are the problems' *what* (ARCHITECTURE §17.6). Which cell a solver fills next,
// the order it tries symbols in and how a validator tracks what it has seen are each strategy's
// *how*, and stay with the solutions; every arm reads the rules here, baselines included (§17.5).
// Solving and generating are algorithms, not the rule: Sudoku is an exact cover problem, whose
// matrix SudokuCover states, and Knuth's Algorithm X with dancing links (Algorithms/ExactCover)
// finds a cover of it. The grid's numbers are static abstract members of a struct, so they fold
// to constants in each caller.
/// <summary>The Sudoku board's size, and the rule a placed symbol obeys, on any grid.</summary>
/// <typeparam name="TGrid">The grid, such as <see cref="ClassicSudoku"/>.</typeparam>
public static class Sudoku<TGrid>
    where TGrid : struct, ISudokuGrid
{
    /// <summary>The number of rows, of columns, of regions and of symbols on the board.</summary>
    public static int Size => TGrid.Order;

    /// <summary>Numbers the region a cell lies in; boxes are numbered row by row.</summary>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <returns>The number of the region holding the cell.</returns>
    public static int RegionOf(int row, int col) => TGrid.RegionOf(row, col);

    /// <summary>
    /// Whether a digit may go in an empty cell: no cell of its row, column or region holds it.
    /// </summary>
    /// <param name="board">The board, one array of <see cref="Size"/> characters per row.</param>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <param name="digit">The digit to place.</param>
    /// <returns><see langword="true"/> when no unit of the cell holds the digit already.</returns>
    public static bool CanPlace(char[][] board, int row, int col, char digit) =>
        !LatinSquare<TGrid>.IsInRowOrColumn(board, row, col, digit) &&
        !TGrid.IsInRegion(board, row, col, digit);

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

    /// <summary>
    /// Whether a board is solved: complete, a Latin square, and no region holding a symbol twice.
    /// </summary>
    /// <param name="board">The board, one array of <see cref="Size"/> characters per row.</param>
    /// <returns>
    /// <see langword="true"/> when every row, column and region holds each symbol once.
    /// </returns>
    public static bool IsSolved(char[][] board)
    {
        if (!IsComplete(board))
        {
            return false;
        }

        if (!LatinSquare<TGrid>.IsLatinSquare(board))
        {
            return false;
        }

        for (var region = 0; region < Size; region++)
        {
            if (HasRepeatInRegion(board, region))
            {
                return false;
            }
        }

        return true;
    }

    // Whether two of a region's cells hold the same symbol. On a Latin square every cell holds one
    // of the n symbols, so a region of n cells without a repeat holds each of them once.
    private static bool HasRepeatInRegion(char[][] board, int region)
    {
        for (var i = 1; i < Size; i++)
        {
            var (row, col) = TGrid.CellOf(region, i);

            if (IsAmongEarlierCells(board, region, i, board[row][col]))
            {
                return true;
            }
        }

        return false;
    }

    // Whether one of the region's cells before the given one holds the symbol.
    private static bool IsAmongEarlierCells(char[][] board, int region, int index, char symbol)
    {
        for (var j = 0; j < index; j++)
        {
            var (row, col) = TGrid.CellOf(region, j);

            if (board[row][col] == symbol)
            {
                return true;
            }
        }

        return false;
    }
}
