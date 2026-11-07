using DSAExperimentation.DataStructures.Alphabets;
using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;

namespace DSAExperimentation.Domain.Games.Sudoku;

// Sudoku on any grid as an exact cover problem. A row of the matrix places one symbol in one cell,
// n x n x n of them, and each placement meets four columns, one from each family of n x n
// constraints: its cell holds a symbol, its row holds the symbol, its column holds it, and its
// region holds it. Every constraint is met exactly once in a solved board, so a cover is a
// solution, and a puzzle's givens are rows chosen before the search (GivenRows). The region family
// reads the grid's partition, so box grids and jigsaw grids reduce alike. A row is numbered
// (cell x n) + symbol, the cell row by row, and symbols are numbered 0 to n - 1; which characters
// write them is the board's notation, an alphabet whose first n letters are the grid's symbols. The
// search that finds a cover is Algorithms/ExactCover's. The struct carries nothing, so default
// is the problem; its numbers are the grid's, static abstract members that fold per grid.
/// <summary>Sudoku as an exact cover: placements as rows, constraints as columns.</summary>
/// <typeparam name="TGrid">The grid, such as <see cref="ClassicSudoku"/>.</typeparam>
public readonly struct SudokuCover<TGrid> : IExactCoverProblem
    where TGrid : struct, ISudokuGrid
{
    // The constraint families, each n x n columns, in the order a placement lists them: a cell
    // holds a symbol, then a row, a column and a region holds each symbol.
    private const int CellFamily = 0;
    private const int RowFamily = 1;
    private const int ColumnFamily = 2;
    private const int RegionFamily = 3;
    private const int Families = RegionFamily + 1;

    private static int Cells => TGrid.Order * TGrid.Order;

    /// <summary>Gets the constraints: four families of n x n.</summary>
    public int PrimaryColumnCount => Families * Cells;

    /// <summary>Gets zero: every Sudoku constraint is met exactly once.</summary>
    public int SecondaryColumnCount => 0;

    /// <summary>Gets the placements, a symbol in a cell: n x n x n.</summary>
    public int RowCount => Cells * TGrid.Order;

    /// <summary>Counts the constraints a placement meets: one of each family, four.</summary>
    /// <param name="row">The placement.</param>
    /// <returns>Four.</returns>
    public int ColumnCountOf(int row) => Families;

    /// <summary>Writes the four constraints a placement meets.</summary>
    /// <param name="row">The placement.</param>
    /// <param name="columns">Four entries: the cell, row, column and region constraints.</param>
    public void WriteColumns(int row, Span<int> columns)
    {
        var (cellRow, cellCol, symbol) = PlacementOf(row);
        var order = TGrid.Order;
        var region = TGrid.RegionOf(cellRow, cellCol);

        columns[CellFamily] = (CellFamily * Cells) + (cellRow * order) + cellCol;
        columns[RowFamily] = (RowFamily * Cells) + (cellRow * order) + symbol;
        columns[ColumnFamily] = (ColumnFamily * Cells) + (cellCol * order) + symbol;
        columns[RegionFamily] = (RegionFamily * Cells) + (region * order) + symbol;
    }

    /// <summary>Numbers the row that places a symbol in a cell.</summary>
    /// <param name="row">The cell's row, from 0.</param>
    /// <param name="col">The cell's column, from 0.</param>
    /// <param name="symbol">The symbol's number, from 0 to n - 1.</param>
    /// <returns>The placement's row of the matrix.</returns>
    public static int PlacementRow(int row, int col, int symbol) =>
        (((row * TGrid.Order) + col) * TGrid.Order) + symbol;

    /// <summary>Finds the cell and symbol a row of the matrix places.</summary>
    /// <param name="placement">The row of the matrix, from 0 below n x n x n.</param>
    /// <returns>The cell's row and column, and the symbol's number.</returns>
    public static (int Row, int Col, int Symbol) PlacementOf(int placement)
    {
        var cell = placement / TGrid.Order;

        return (cell / TGrid.Order, cell % TGrid.Order, placement % TGrid.Order);
    }

    /// <summary>Reads a board's givens as the placements a cover must contain.</summary>
    /// <typeparam name="TSymbols">An alphabet whose first n letters write the symbols.</typeparam>
    /// <param name="board">The board, one array of n characters per row.</param>
    /// <returns>One placement for each cell holding a symbol, row by row.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A cell holds a character that is neither the empty-cell mark nor one of the grid's symbols.
    /// </exception>
    public static int[] GivenRows<TSymbols>(char[][] board)
        where TSymbols : struct, IAlphabet
    {
        var givens = new List<int>();

        for (var row = 0; row < TGrid.Order; row++)
        {
            for (var col = 0; col < TGrid.Order; col++)
            {
                if (board[row][col] == TGrid.EmptyCell)
                {
                    continue;
                }

                var symbol = SymbolOf<TSymbols>(board[row][col]);
                var placement = PlacementRow(row, col, symbol);
                givens.Add(placement);
            }
        }

        return [.. givens];
    }

    /// <summary>Writes a cover's placements onto a board.</summary>
    /// <typeparam name="TSymbols">An alphabet whose first n letters write the symbols.</typeparam>
    /// <param name="board">The board, one array of n characters per row.</param>
    /// <param name="cover">The placements, such as a cover of this problem.</param>
    public static void Fill<TSymbols>(char[][] board, ReadOnlySpan<int> cover)
        where TSymbols : struct, IAlphabet
    {
        foreach (var placement in cover)
        {
            var (row, col, symbol) = PlacementOf(placement);
            board[row][col] = TSymbols.LetterAt(symbol);
        }
    }

    // A symbol's number: its letter's slot, which must be among the alphabet's first n.
    private static int SymbolOf<TSymbols>(char letter)
        where TSymbols : struct, IAlphabet
    {
        var symbol = TSymbols.IndexOf(letter);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(symbol, TGrid.Order, nameof(letter));

        return symbol;
    }
}
