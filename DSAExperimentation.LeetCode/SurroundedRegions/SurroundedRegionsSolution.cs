using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.SurroundedRegions;

// LeetCode 130. Surrounded Regions: flip every 'O' region that has no path
// to the border into 'X', in place.
//
// A region survives exactly when it is reachable from a border cell, so the
// puzzle inverts into "mark everything reachable from the border, then flip
// whatever wasn't marked" - one DFS per border cell (most of which are
// no-ops on cells already 'X' or already marked) instead of a flood fill
// starting from every interior region.
internal static class SurroundedRegionsSolution
{
    private const char Open = 'O';
    private const char Captured = 'X';
    private const char BorderConnected = '#';

    public static void SolveByBorderDepthFirstSearch(char[][] board)
    {
        var rows = board.Length;
        var cols = board[0].Length;
        MarkBorderReachable(board, rows, cols);

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
