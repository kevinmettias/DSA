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

        void Mark((int Row, int Col) start)
        {
            if (board[start.Row][start.Col] != Open)
            {
                return;
            }

            foreach (var (row, col) in DepthFirstSearch.Traverse(start, Neighbors))
            {
                board[row][col] = BorderConnected;
            }
        }

        IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p)
        {
            (int Row, int Col)[] next = [(p.Row + 1, p.Col), (p.Row - 1, p.Col), (p.Row, p.Col + 1), (p.Row, p.Col - 1)];

            foreach (var n in next)
            {
                if (n.Row >= 0 && n.Row < rows && n.Col >= 0 && n.Col < cols && board[n.Row][n.Col] == Open)
                {
                    yield return n;
                }
            }
        }

        for (var r = 0; r < rows; r++)
        {
            Mark((r, 0));
            Mark((r, cols - 1));
        }

        for (var c = 0; c < cols; c++)
        {
            Mark((0, c));
            Mark((rows - 1, c));
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                board[r][c] = board[r][c] == BorderConnected ? Open : Captured;
            }
        }
    }
}
