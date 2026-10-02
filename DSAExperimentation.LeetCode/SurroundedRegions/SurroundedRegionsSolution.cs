using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.SurroundedRegions;

// LeetCode 130. Surrounded Regions: flip every 'O' region that has no path
// to the border into 'X', in place.
//
// A region survives exactly when it is reachable from a border cell, so the
// puzzle inverts into "mark everything reachable from the border, then flip
// whatever wasn't marked" - one flood fill per border cell (most of which are
// no-ops on cells already 'X' or already marked) instead of a flood fill
// starting from every interior region.
//
// Two arms share that inversion and that flip: the depth-first search below, which
// expands each border flood through DepthFirstSearch.Traverse's recursive walk, and
// the breadth-first search above it, which drains an explicit Queue. They write the
// same marker to the same cells and then share one capture pass, so the boards they
// leave are identical.
internal static class SurroundedRegionsSolution
{
    private const char Open = 'O';
    private const char Captured = 'X';
    private const char BorderConnected = '#';

    // The explicit-queue counterpart to the depth-first capture below: the same border
    // marking and the same in-place flip, but each border start drains a Queue in FIFO
    // order instead of recursing, so a large region cannot deepen the call stack. It
    // marks the same cells with the same BorderConnected marker, so both arms must
    // leave the board identical.
    public static void SolveByBorderBreadthFirstSearch(char[][] board)
    {
        var rows = board.Length;
        var cols = board[0].Length;
        MarkBorderReachableBreadthFirst(board, rows, cols);
        CaptureUnmarked(board, rows, cols);
    }

    public static void SolveByBorderDepthFirstSearch(char[][] board)
    {
        var rows = board.Length;
        var cols = board[0].Length;
        MarkBorderReachable(board, rows, cols);
        CaptureUnmarked(board, rows, cols);
    }

    // Border-reachable cells are left Open; every other cell becomes Captured, which
    // also turns the temporary marker back into an 'O'. The two arms differ only in how
    // they mark, so this shared pass is the same for both.
    private static void CaptureUnmarked(char[][] board, int rows, int cols)
    {
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var survives = board[r][c] == BorderConnected;
                board[r][c] = survives ? Open : Captured;
            }
        }
    }

    // Every border cell is a way into a surviving region, so walking outward from
    // each of them marks exactly the 'O' cells that reach the border - the
    // complement of what the flip above captures.
    private static void MarkBorderReachable(char[][] board, int rows, int cols)
    {
        for (var r = 0; r < rows; r++)
        {
            MarkBorderConnected(board, (r, 0), rows, cols);
            MarkBorderConnected(board, (r, cols - 1), rows, cols);
        }

        for (var c = 0; c < cols; c++)
        {
            MarkBorderConnected(board, (0, c), rows, cols);
            MarkBorderConnected(board, (rows - 1, c), rows, cols);
        }
    }

    // One entry point: a cell that is not an un-marked 'O' has nothing new behind
    // it, and otherwise the cell and everything reachable from it get marked.
    private static void MarkBorderConnected(char[][] board, (int Row, int Col) start, int rows, int cols)
    {
        if (board[start.Row][start.Col] != Open)
        {
            return;
        }

        foreach (var (row, col) in DepthFirstSearch.Traverse(start, cell => OpenNeighbors(board, cell, rows, cols)))
        {
            board[row][col] = BorderConnected;
        }
    }

    // The breadth-first twin of MarkBorderReachable: the same walk over every border
    // cell, but each entry point drains a queue rather than recursing.
    private static void MarkBorderReachableBreadthFirst(char[][] board, int rows, int cols)
    {
        for (var r = 0; r < rows; r++)
        {
            MarkBorderConnectedBreadthFirst(board, (r, 0), rows, cols);
            MarkBorderConnectedBreadthFirst(board, (r, cols - 1), rows, cols);
        }

        for (var c = 0; c < cols; c++)
        {
            MarkBorderConnectedBreadthFirst(board, (0, c), rows, cols);
            MarkBorderConnectedBreadthFirst(board, (rows - 1, c), rows, cols);
        }
    }

    // Marks a start and everything reachable from it through un-marked 'O' cells,
    // claiming each such cell before it is queued so the frontier never carries a cell
    // twice. A start that is not an un-marked 'O' has nothing new behind it.
    private static void MarkBorderConnectedBreadthFirst(
        char[][] board, (int Row, int Col) start, int rows, int cols)
    {
        if (board[start.Row][start.Col] != Open)
        {
            return;
        }

        var frontier = new Queue<(int Row, int Col)>();
        board[start.Row][start.Col] = BorderConnected;
        frontier.Enqueue(start);

        while (frontier.Count > 0)
        {
            foreach (var neighbor in OpenNeighbors(board, frontier.Dequeue(), rows, cols))
            {
                board[neighbor.Row][neighbor.Col] = BorderConnected;
                frontier.Enqueue(neighbor);
            }
        }
    }

    // The four orthogonal neighbours of a cell that are still un-marked 'O's.
    private static IEnumerable<(int Row, int Col)> OpenNeighbors(
        char[][] board, (int Row, int Col) cell, int rows, int cols)
    {
        (int Row, int Col)[] next = [(cell.Row + 1, cell.Col), (cell.Row - 1, cell.Col), (cell.Row, cell.Col + 1), (cell.Row, cell.Col - 1)];

        foreach (var n in next)
        {
            if (IsInsideGrid(n.Row, n.Col, rows, cols) && board[n.Row][n.Col] == Open)
            {
                yield return n;
            }
        }
    }

    // Both coordinates have to land inside the board for a neighbour to be worth
    // visiting at all.
    private static bool IsInsideGrid(int row, int col, int rows, int cols) =>
        row >= 0 && row < rows && col >= 0 && col < cols;
}
