namespace DSAExperimentation.Domain.Games.SlidingPuzzle;

// What every sliding puzzle does, for the board TBoard names. A board is spelled row by row, one
// symbol per cell: '0' for the empty cell and '1', '2', ... for the tiles, then 'A' onward past 9,
// so boards of up to 36 cells are spelled. A move slides a tile next to the empty cell into it,
// which is the empty cell trading places with a neighbour, and the puzzle is solved when the tiles
// read in order with the empty cell last.
//
// Which arrangements can be solved is the puzzle's parity law, which holds for every rectangular
// board. A slide along a row leaves the tiles' order, so the number of pairs out of order (the
// inversions) is unchanged; a slide along a column moves one tile past Columns - 1 others. On a
// board an odd number of cells wide that is an even change, so the inversions' parity never changes,
// and an arrangement is solvable exactly when its inversions are even, as the solved board's are. On
// an even width each column move flips the inversions' parity and moves the empty cell one row, so
// inversions plus the empty cell's row, counted from the bottom, keeps its parity: an arrangement is
// solvable exactly when that sum is odd, as the solved board's 0 + 1 is. Half of all arrangements are
// solvable either way.
/// <summary>The arrangements of a sliding puzzle, the slides between them, and which can be solved.</summary>
/// <typeparam name="TBoard">The board's shape.</typeparam>
public static class SlidingPuzzle<TBoard>
    where TBoard : struct, IPuzzleBoard
{
    /// <summary>The empty cell's symbol.</summary>
    public const char Empty = '0';

    // A cell's symbol by what it holds: the empty cell, then the tiles in order.
    private const string Symbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    // The most cells the empty cell can trade places with: one each way along a row and a column.
    private const int MostNeighbours = 4;

    // The cells a tile can slide into the empty cell from, by the empty cell's index: worked out once
    // per board, as a table like the one LC 773's baseline keeps for its own.
    private static readonly int[][] SourcesByEmptyCell = [.. Enumerable.Range(0, CellCount).Select(NeighboursOf)];

    /// <summary>How many cells the board has.</summary>
    public static int CellCount => TBoard.Rows * TBoard.Columns;

    /// <summary>The most arrangements one slide from any arrangement.</summary>
    public static int MostSlides => MostNeighbours;

    /// <summary>The solved board: the tiles in order, row by row, and the empty cell last.</summary>
    public static string Solved { get; } = Symbols[1..CellCount] + Empty;

    /// <summary>The board's symbols in order: the empty cell's, then each tile's.</summary>
    public static string CellSymbols { get; } = Symbols[..CellCount];

    /// <summary>
    /// The cells a tile can slide into the empty cell from: its neighbours above, beside and below,
    /// in cell order.
    /// </summary>
    /// <param name="empty">The empty cell's index, reading the board row by row.</param>
    /// <returns>Two cells in a corner, three on an edge, four inside.</returns>
    public static ReadOnlySpan<int> SlideSources(int empty) => SourcesByEmptyCell[empty];

    /// <summary>Writes the arrangements one slide from another, in the order their tiles' cells read.</summary>
    /// <param name="board">The arrangement slid from, spelled row by row.</param>
    /// <param name="into">Where to write them, at least <see cref="MostSlides"/> long.</param>
    /// <returns>How many were written.</returns>
    public static int WriteOneSlideAway(string board, Span<string> into)
    {
        var empty = board.IndexOf(Empty);
        var sources = SourcesByEmptyCell[empty];

        for (var i = 0; i < sources.Length; i++)
        {
            into[i] = Traded(board, empty, sources[i]);
        }

        return sources.Length;
    }

    private static string Traded(string board, int empty, int source)
        => string.Create(board.Length, (board, empty, source), static (cells, slide) =>
        {
            slide.board.AsSpan().CopyTo(cells);
            (cells[slide.empty], cells[slide.source]) = (cells[slide.source], cells[slide.empty]);
        });

    /// <summary>Tells whether slides can bring an arrangement to <see cref="Solved"/>, by the parity law.</summary>
    /// <param name="board">An arrangement, spelled row by row.</param>
    /// <returns>True when the arrangement can be solved.</returns>
    public static bool IsSolvable(string board)
    {
        var inversions = InversionCount(board);

        if (int.IsOddInteger(TBoard.Columns))
        {
            return int.IsEvenInteger(inversions);
        }

        var emptyRowFromBottom = TBoard.Rows - (board.IndexOf(Empty) / TBoard.Columns);

        return int.IsOddInteger(inversions + emptyRowFromBottom);
    }

    /// <summary>Counts the pairs of tiles out of order, the empty cell left out.</summary>
    /// <param name="board">An arrangement, spelled row by row.</param>
    /// <returns>How many pairs of tiles read in the wrong order.</returns>
    public static int InversionCount(string board)
    {
        var inversions = 0;

        for (var first = 0; first < board.Length; first++)
        {
            for (var second = first + 1; second < board.Length; second++)
            {
                if (IsOutOfOrder(board[first], board[second]))
                {
                    inversions++;
                }
            }
        }

        return inversions;
    }

    private static bool IsOutOfOrder(char first, char second)
        => first != Empty && second != Empty && Symbols.IndexOf(first) > Symbols.IndexOf(second);

    // A cell's neighbours above, to the left, to the right and below, those on the board kept.
    private static int[] NeighboursOf(int cell)
    {
        var (row, column) = Math.DivRem(cell, TBoard.Columns);
        int[] neighbours = [cell - TBoard.Columns, cell - 1, cell + 1, cell + TBoard.Columns];
        bool[] onBoard = [row > 0, column > 0, column < TBoard.Columns - 1, row < TBoard.Rows - 1];

        return [.. neighbours.Where((_, side) => onBoard[side])];
    }
}
