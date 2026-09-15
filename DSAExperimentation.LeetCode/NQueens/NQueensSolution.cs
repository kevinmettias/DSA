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

        SearchRows(0, placed, (cols, diag, anti), results);

        return results;
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
            s => s.Row == n ? NoCandidates() : CandidateColumns(s, n),
            (s, col) => s.Place(col),
            (s, col) => s.Remove(col),
            s => results.Add(s.Board()));

        return results;
    }

    // The candidate columns a state with every row already placed still offers: none.
    // That empty answer is how Backtrack.Search learns the board is complete, not a
    // failure, so it is a value the selector must be able to answer with.
    private static IEnumerable<int> NoCandidates() => [];

    // The columns a row can still take: every column no already-placed queen attacks.
    private static IEnumerable<int> CandidateColumns(QueensState state, int n) =>
        Enumerable.Range(0, n).Where(state.CanPlace);

    // One row of the recursion: `row` is the row being decided, `placed` holds the
    // column every earlier row chose (so its length is the board's own size),
    // `occupancy` is what those queens already attack, and `results` collects whole
    // boards as they complete. Its own recursive call is a second caller of it, so
    // call order puts it with the file's other shared helpers rather than directly
    // beneath SolveByRecursiveDfs.
    private static void SearchRows(
        int row, int[] placed, (bool[] Cols, bool[] Diag, bool[] Anti) occupancy, List<List<string>> results)
    {
        var (cols, diag, anti) = occupancy;
        var n = placed.Length;

        if (row == n)
        {
            var board = Board(n, placed);
            results.Add(board);
            return;
        }

        for (var col = 0; col < n; col++)
        {
            if (IsAttacked(occupancy, row, col, n))
            {
                continue;
            }

            placed[row] = col;
            cols[col] = diag[row - col + n - 1] = anti[row + col] = true;
            SearchRows(row + 1, placed, occupancy, results);
            cols[col] = diag[row - col + n - 1] = anti[row + col] = false;
        }
    }

    // The three occupancy arrays are one idea - what the queens already placed
    // attack - so the baseline's column/diagonal/anti-diagonal test reads as one
    // question instead of three index expressions.
    private static bool IsAttacked((bool[] Cols, bool[] Diag, bool[] Anti) occupancy, int row, int col, int n)
        => occupancy.Cols[col]
            || occupancy.Diag[row - col + n - 1]
            || occupancy.Anti[row + col];

    private static List<string> Board(int n, int[] placed) =>
        Enumerable.Range(0, n)
            .Select(row => BoardRow(n, placed[row]))
            .ToList();

    // One row as the puzzle prints it, given the column that row's queen holds.
    private static string BoardRow(int n, int queenColumn) =>
        new string(Enumerable.Range(0, n).Select(col => col == queenColumn ? 'Q' : '.').ToArray());

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
                .Select(row => BoardRow(n, _placed[row]))
                .ToList();
    }
}
