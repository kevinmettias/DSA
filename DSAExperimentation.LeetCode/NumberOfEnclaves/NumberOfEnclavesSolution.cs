using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.NumberOfEnclaves;

// LeetCode 1020. Number of Enclaves: how many land cells can never walk off the
// grid, moving 4-directionally through land only.
//
// The question inverts: a land cell escapes exactly when its component touches
// the border, so both strategies sink every border-connected component and count
// whatever land survives. Sinking is destructive, so each strategy clones the
// grid it is handed first - the same clone-before-mutating shape
// MaxAreaOfIslandSolution uses, which keeps a caller's own grid (or a benchmark
// fixture reused across iterations) intact.
internal static class NumberOfEnclavesSolution
{
    private const int Land = 1;
    private const int Water = 0;

    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    // The textbook answer: a hand-specialized recursive flood fill from every
    // border cell. Deliberately written without this repo's traversal primitive -
    // it is the arm the composed solution below has to justify itself against.
    public static int NumEnclavesByNaiveFloodFill(int[][] grid)
    {
        var working = CloneGrid(grid);
        var rows = working.Length;
        var cols = working[0].Length;

        foreach (var (row, col) in BorderCells(rows, cols))
        {
            Flood(working, row, col, rows, cols);
        }

        return CountLand(working);
    }

    private static void Flood(int[][] grid, int row, int col, int rows, int cols)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols || grid[row][col] != Land)
        {
            return;
        }

        grid[row][col] = Water;

        Flood(grid, row + 1, col, rows, cols);
        Flood(grid, row - 1, col, rows, cols);
        Flood(grid, row, col + 1, rows, cols);
        Flood(grid, row, col - 1, rows, cols);
    }

    // This repo's own DFS: DepthFirstSearch.Traverse walks one border-connected
    // land component at a time and hands back every cell in it, which is then
    // sunk in place - the same flood-fill composition MaxAreaOfIsland and
    // SurroundedRegions already use for LC 695/130.
    public static int NumEnclavesByDepthFirstSearch(int[][] grid)
    {
        var working = CloneGrid(grid);
        var rows = working.Length;
        var cols = working[0].Length;

        foreach (var (row, col) in BorderCells(rows, cols))
        {
            SinkComponent(working, row, col);
        }

        return CountLand(working);
    }

    private static void SinkComponent(int[][] grid, int startRow, int startCol)
    {
        if (grid[startRow][startCol] != Land)
        {
            return;
        }

        var component = DepthFirstSearch.Traverse((startRow, startCol), p => LandNeighbors(grid, p));

        foreach (var (row, col) in component)
        {
            grid[row][col] = Water;
        }
    }

    private static IEnumerable<(int Row, int Col)> LandNeighbors(int[][] grid, (int Row, int Col) p)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: p.Row + dRow, Col: p.Col + dCol);

            if (IsLand(next, rows, cols, grid))
            {
                yield return next;
            }
        }
    }

    private static bool IsLand((int Row, int Col) next, int rows, int cols, int[][] grid)
    {
        if (next.Row < 0 || next.Row >= rows || next.Col < 0 || next.Col >= cols)
        {
            return false;
        }

        return grid[next.Row][next.Col] == Land;
    }

    private static IEnumerable<(int Row, int Col)> BorderCells(int rows, int cols)
    {
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (r == 0 || r == rows - 1 || c == 0 || c == cols - 1)
                {
                    yield return (r, c);
                }
            }
        }
    }

    private static int CountLand(int[][] grid)
    {
        var count = 0;

        foreach (var row in grid)
        {
            foreach (var cell in row)
            {
                if (cell == Land)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int[][] CloneGrid(int[][] grid)
    {
        var clone = new int[grid.Length][];

        for (var r = 0; r < grid.Length; r++)
        {
            clone[r] = (int[])grid[r].Clone();
        }

        return clone;
    }
}
