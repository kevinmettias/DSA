namespace DSAExperimentation.LeetCode.CreateGridWithExactlyKPathsI;

// LeetCode 3988. Create Grid With Exactly K Paths I: build any m x n grid of
// free ('.') and obstacle ('#') cells so the number of monotone (right/down)
// paths from (0, 0) to (m - 1, n - 1) is exactly k, or report that no such grid
// exists. k is tiny (1..4), so this never needs to search the space of grids -
// only the space of ONE fully-open a x b rectangle anchored at (0, 0), whose own
// path count is the closed-form C(a + b - 2, a - 1). Whichever (a, b) hits k
// becomes the grid: the rectangle itself, plus a single-width corridor from its
// bottom-right corner out to (m - 1, n - 1). That corner is a cut vertex (every
// free cell outside the rectangle is reachable only through it), so the total
// path count is exactly the rectangle's own count times the corridor's one path.
//
// A plain rectangle can only ever produce values of the form C(a + b - 2, a - 1)
// - 1, 2, 3, 4, 6, ... - which skips nothing in {1, 2, 3, 4} except in one shape:
// no a x b rectangle equals 4 when both a < 4 and b < 4 (the smallest is 2x4 or
// 4x2). For exactly that corner case (m = n = 3), two 2x2 rectangles chained so
// the first one's bottom-right corner IS the second one's top-left corner
// multiply instead of needing extra room: 2 x 2 = 4 paths in a 3x3 footprint.
//
// Both strategies build the identical grid; they differ only in how they decide
// a rectangle's path count while searching for one that equals k.
internal static class CreateGridWithExactlyKPathsISolution
{
    private const char FreeCell = '.';
    private const char ObstacleCell = '#';
    private const int DoubleChainSide = 3;

    // The double-chain interior: two overlapping 2x2 open rectangles sharing the
    // corner (1, 1), giving 2 x 2 = 4 paths in a 3x3 footprint - see class comment.
    private static readonly (int Row, int Col)[] DoubleChainFreeCells =
        [(0, 0), (0, 1), (1, 0), (1, 1), (1, 2), (2, 1), (2, 2)];

    // The straightforward arm: a rectangle's path count computed the way most
    // people would reach for first, C(n, r) via Pascal's-triangle-style DP over
    // the candidate rectangle itself rather than a combinatorial identity.
    public static string[] CreateGridByPathCountDp(int m, int n, int k) => TryBuild(m, n, k, DpPathCount);

    // The composed arm: the same search, but a rectangle's path count comes from
    // the closed-form binomial coefficient instead of simulating the rectangle.
    public static string[] CreateGridByBinomialFormula(int m, int n, int k) => TryBuild(m, n, k, BinomialPathCount);

    private static string[] TryBuild(int m, int n, int k, Func<int, int, int> rectanglePathCount)
    {
        for (var a = 1; a <= m; a++)
        {
            for (var b = 1; b <= n; b++)
            {
                if (rectanglePathCount(a, b) == k)
                {
                    return BuildRectangleGrid(m, n, a, b);
                }
            }
        }

        return k == 4 && m >= DoubleChainSide && n >= DoubleChainSide
            ? BuildDoubleChainGrid(m, n)
            : [];
    }

    // Unique-paths DP over a fully open a x b rectangle: dp[j] is the path count
    // to column j of the current row, updated in place row by row.
    private static int DpPathCount(int a, int b)
    {
        var dp = new int[b];
        Array.Fill(dp, 1);

        for (var i = 1; i < a; i++)
        {
            for (var j = 1; j < b; j++)
            {
                dp[j] += dp[j - 1];
            }
        }

        return dp[b - 1];
    }

    // C(a + b - 2, a - 1), the number of monotone paths across a fully open a x b
    // rectangle: choose which a - 1 of the a + b - 2 moves are "down". Computed
    // incrementally (multiply then divide at each step) so every partial result
    // stays an exact integer - values here never exceed C(18, 9), nowhere near
    // overflowing a long.
    private static int BinomialPathCount(int a, int b)
    {
        var totalMoves = a + b - 2;
        var downMoves = a - 1;
        var result = 1L;

        for (var i = 1; i <= downMoves; i++)
        {
            result = result * (totalMoves - downMoves + i) / i;
        }

        return (int)result;
    }

    private static string[] BuildRectangleGrid(int m, int n, int a, int b)
    {
        var grid = BlankGrid(m, n);

        for (var i = 0; i < a; i++)
        {
            for (var j = 0; j < b; j++)
            {
                grid[i][j] = FreeCell;
            }
        }

        FreeCorridorFromCorner(grid, cornerRow: a - 1, cornerCol: b - 1);

        return ToRows(grid);
    }

    private static string[] BuildDoubleChainGrid(int m, int n)
    {
        var grid = BlankGrid(m, n);

        foreach (var (row, col) in DoubleChainFreeCells)
        {
            grid[row][col] = FreeCell;
        }

        FreeCorridorFromCorner(grid, cornerRow: DoubleChainSide - 1, cornerCol: DoubleChainSide - 1);

        return ToRows(grid);
    }

    // The single-width path from a rectangle's bottom-right corner out to the
    // grid's own bottom-right corner: right along the corner's row, then down
    // along the grid's last column. Contributes exactly one path on top of
    // whatever the rectangle itself already counts.
    private static void FreeCorridorFromCorner(char[][] grid, int cornerRow, int cornerCol)
    {
        var m = grid.Length;
        var n = grid[0].Length;

        for (var j = cornerCol; j < n; j++)
        {
            grid[cornerRow][j] = FreeCell;
        }

        for (var i = cornerRow; i < m; i++)
        {
            grid[i][n - 1] = FreeCell;
        }
    }

    private static char[][] BlankGrid(int m, int n)
    {
        var grid = new char[m][];

        for (var i = 0; i < m; i++)
        {
            grid[i] = new char[n];
            Array.Fill(grid[i], ObstacleCell);
        }

        return grid;
    }

    private static string[] ToRows(char[][] grid) => [.. grid.Select(row => new string(row))];
}
