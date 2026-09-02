using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.NQueensII;

// LeetCode 52. N-Queens II: count placements of n non-attacking queens on an
// n x n board (one per row and column, no two sharing a diagonal), without
// ever materializing a board - the only thing distinguishing this from LC
// 51's placement-listing shape.
internal static class NQueensIISolution
{
    private const int DiagonalArrayMultiplier = 2;

    // The textbook answer: hand-rolled recursion over three bool[] occupancy
    // arrays (columns, "/" diagonals, "\" anti-diagonals), written without
    // this repo's Backtrack.Search - the arm the composed solution below has
    // to justify itself against.
    public static int TotalNQueensByArrayRecursion(int n)
    {
        var columns = new bool[n];
        var diagonals = new bool[(DiagonalArrayMultiplier * n) - 1];
        var antiDiagonals = new bool[(DiagonalArrayMultiplier * n) - 1];
        var count = 0;

        void SearchRow(int row)
        {
            if (row == n)
            {
                count++;
                return;
            }

            for (var col = 0; col < n; col++)
            {
                if (columns[col] || diagonals[row - col + n - 1] || antiDiagonals[row + col])
                {
                    continue;
                }

                columns[col] = diagonals[row - col + n - 1] = antiDiagonals[row + col] = true;
                SearchRow(row + 1);
                columns[col] = diagonals[row - col + n - 1] = antiDiagonals[row + col] = false;
            }
        }

        SearchRow(0);
        return count;
    }

    // This repo's own Backtrack.Search: BoardState's column/diagonal/
    // anti-diagonal occupancy is the choose/unchoose contract, Candidates is
    // every open column in the current row, and OnSolution just counts
    // rather than recording a board - the same primitive LC 51 (N-Queens)
    // composes to list boards instead.
    public static int TotalNQueensByBacktrackSearch(int n)
    {
        var count = 0;
        var state = new BoardState(n);

        Backtrack.Search<BoardState, int>(
            state,
            s => s.Row == n,
            s => s.Row == n ? [] : Enumerable.Range(0, n).Where(s.CanPlace),
            (s, col) => s.Place(col),
            (s, col) => s.Remove(col),
            _ => count++);

        return count;
    }

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
