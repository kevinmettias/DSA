using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.NQueens;

// LeetCode 51. N-Queens: every distinct placement of n non-attacking queens on an
// n x n board, reported as row strings ('Q'/'.').
//
// Two strategies over the same column/diagonal occupancy state: a hand-rolled
// recursive DFS (the textbook baseline) and this repo's own Backtrack.Search engine
// driving row-by-row placement.
internal static class NQueensSolution
{
    private const int DiagonalArrayMultiplier = 2;

    // The textbook baseline: plain recursion over three occupancy arrays, choosing
    // and unchoosing a column by hand. Deliberately written without this repo's
    // primitives - it is the arm the composed solution below has to justify itself
    // against.
    public static List<List<string>> SolveByRecursiveDfs(int n)
    {
        var results = new List<List<string>>();
        var placed = new int[n];
        var cols = new bool[n];
        var diag = new bool[(DiagonalArrayMultiplier * n) - 1];
        var anti = new bool[(DiagonalArrayMultiplier * n) - 1];

        Search(0);
        return results;

        void Search(int row)
        {
            if (row == n)
            {
                results.Add(Board(n, placed));
                return;
            }

            for (var col = 0; col < n; col++)
            {
                if (cols[col] || diag[row - col + n - 1] || anti[row + col])
                {
                    continue;
                }

                placed[row] = col;
                cols[col] = diag[row - col + n - 1] = anti[row + col] = true;
                Search(row + 1);
                cols[col] = diag[row - col + n - 1] = anti[row + col] = false;
            }
        }
    }

    // This repo's own backtracking engine: row-by-row queen placement with
    // column/diagonal occupancy state is exactly Backtrack.Search's
    // choose/explore/unchoose shape over one shared mutable board.
    public static List<List<string>> SolveByBacktrackEngine(int n)
    {
        var results = new List<List<string>>();
        var state = new QueensState(n);

        Backtrack.Search<QueensState, int>(
            state,
            s => s.Row == n,
            s => s.Row == n ? [] : Enumerable.Range(0, n).Where(s.CanPlace),
            (s, col) => s.Place(col),
            (s, col) => s.Remove(col),
            s => results.Add(s.Board()));

        return results;
    }

    private static List<string> Board(int n, int[] placed) =>
        Enumerable.Range(0, n)
            .Select(row => new string(Enumerable.Range(0, n).Select(col => placed[row] == col ? 'Q' : '.').ToArray()))
            .ToList();

    private sealed class QueensState(int n)
    {
        private readonly bool[] _cols = new bool[n];
        private readonly bool[] _diag = new bool[(DiagonalArrayMultiplier * n) - 1];
        private readonly bool[] _anti = new bool[(DiagonalArrayMultiplier * n) - 1];
        private readonly int[] _placed = new int[n];

        public int Row { get; private set; }

        public bool CanPlace(int col) => !_cols[col] && !_diag[Row - col + n - 1] && !_anti[Row + col];

        public void Place(int col)
        {
            _placed[Row] = col;
            _cols[col] = _diag[Row - col + n - 1] = _anti[Row + col] = true;
            Row++;
        }

        public void Remove(int col)
        {
            Row--;
            _cols[col] = _diag[Row - col + n - 1] = _anti[Row + col] = false;
        }

        public List<string> Board() =>
            Enumerable.Range(0, n)
                .Select(row => new string(Enumerable.Range(0, n).Select(col => _placed[row] == col ? 'Q' : '.').ToArray()))
                .ToList();
    }
}
