using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.NQueensII;

// LeetCode 52. N-Queens II: count the placements of `boardSize` non-attacking
// queens on a `boardSize` x `boardSize` board (one per row and column, no two
// sharing a diagonal), without ever materializing a board - the only thing
// distinguishing this from LC 51's placement-listing shape.
internal static class NQueensIISolution
{
    private const int DiagonalArrayMultiplier = 2;

    // The textbook answer: hand-rolled recursion over three bool[] occupancy
    // arrays (columns, "/" diagonals, "\" anti-diagonals), written without
    // this repo's Backtrack.Search - the arm the composed solution below has
    // to justify itself against.
    public static int TotalNQueensByArrayRecursion(int boardSize)
    {
        var columns = new bool[boardSize];
        var diagonals = new bool[(DiagonalArrayMultiplier * boardSize) - 1];
        var antiDiagonals = new bool[(DiagonalArrayMultiplier * boardSize) - 1];
        var board = (Columns: columns, Diagonals: diagonals, AntiDiagonals: antiDiagonals);

        return CountFrom(board, 0);
    }

    // This repo's own Backtrack.Search: BoardState's column/diagonal/
    // anti-diagonal occupancy is the choose/unchoose contract, Candidates is
    // every open column in the current row, and OnSolution just counts
    // rather than recording a board - the same primitive LC 51 (N-Queens)
    // composes to list boards instead.
    public static int TotalNQueensByBacktrackSearch(int boardSize)
    {
        var count = 0;
        var state = new BoardState(boardSize);

        Backtrack.Search<BoardState, int>(
            state,
            s => s.Row == boardSize,
            s => s.Row == boardSize ? Array.Empty<int>() : OpenColumns(s, boardSize),
            (s, col) => s.Place(col),
            (s, col) => s.Remove(col),
            _ => count++);

        return count;
    }

    // The columns still open in the row being decided. Read lazily, so each Remove is
    // seen reopening a column before the next candidate is asked for.
    private static IEnumerable<int> OpenColumns(BoardState state, int columnCount) =>
        Enumerable.Range(0, columnCount).Where(state.CanPlace);

    // How many placements complete the board from `row` down, given the occupancy
    // the caller has already established. Getting past the last row means every row
    // placed a queen, so that one descent counts as a completed board.
    private static int CountFrom((bool[] Columns, bool[] Diagonals, bool[] AntiDiagonals) board, int row)
    {
        var n = board.Columns.Length;
        if (row == n)
        {
            return 1;
        }

        var count = 0;
        for (var col = 0; col < n; col++)
        {
            count += PlaceColumn(board, row, col);
        }

        return count;
    }

    // One column at one row: a queen may stand there only when the column and both
    // diagonals through it are free, and a free square contributes however many
    // placements complete the board below it. The square is cleared again so the
    // caller's loop can go on to the next column.
    private static int PlaceColumn((bool[] Columns, bool[] Diagonals, bool[] AntiDiagonals) board, int row, int col)
    {
        var n = board.Columns.Length;
        var diagonal = row - col + n - 1;
        var antiDiagonal = row + col;

        if (board.Columns[col] || IsOnEitherDiagonal(board.Diagonals, diagonal, board.AntiDiagonals, antiDiagonal))
        {
            return 0;
        }

        board.Columns[col] = board.Diagonals[diagonal] = board.AntiDiagonals[antiDiagonal] = true;
        var count = CountFrom(board, row + 1);
        board.Columns[col] = board.Diagonals[diagonal] = board.AntiDiagonals[antiDiagonal] = false;
        return count;
    }

    // A square is already attacked when either of the two diagonals through it holds
    // a queen - the "/" diagonal indexed one way, the "\" anti-diagonal the other.
    private static bool IsOnEitherDiagonal(bool[] diagonals, int diagonal, bool[] antiDiagonals, int antiDiagonal) =>
        diagonals[diagonal] || antiDiagonals[antiDiagonal];

    // Bespoke to this problem: tracks column/diagonal/anti-diagonal
    // occupancy for Backtrack.Search's choose/unchoose contract. Meaningless
    // outside an N-Queens-shaped board, so it stays nested here rather than
    // moving to DataStructures/ or Algorithms/ (ARCHITECTURE.md §17.3).
    private sealed class BoardState(int n)
    {
        private readonly bool[] _columns = new bool[n];
        private readonly bool[] _diagonals = new bool[(DiagonalArrayMultiplier * n) - 1];
        private readonly bool[] _antiDiagonals = new bool[(DiagonalArrayMultiplier * n) - 1];

        public int Row { get; private set; }

        public bool CanPlace(int col) =>
            !_columns[col] && !_diagonals[Row - col + n - 1] && !_antiDiagonals[Row + col];

        public void Place(int col)
        {
            _columns[col] = _diagonals[Row - col + n - 1] = _antiDiagonals[Row + col] = true;
            Row++;
        }

        public void Remove(int col)
        {
            Row--;
            _columns[col] = _diagonals[Row - col + n - 1] = _antiDiagonals[Row + col] = false;
        }
    }
}
