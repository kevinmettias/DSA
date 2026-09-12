using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.MaxAreaOfIsland;

// LeetCode 695. Max Area of Island: flood-fill each unvisited land cell's
// connected component under 4-directional adjacency and keep the largest area
// seen. Both strategies visit cells by zeroing them in place, so each strategy
// clones the grid it is handed first - the same clone-before-mutating shape the
// original benchmark arms used, now made unconditional so a caller's own grid (or
// a benchmark fixture reused across iterations) is never left half-zeroed.
internal static class MaxAreaOfIslandSolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    // The textbook answer: a hand-specialized recursive flood fill, BCL only.
    // Deliberately written without this repo's traversal primitive - it is the arm
    // the composed solution below has to justify itself against.
    public static int MaxAreaByNaiveFloodFill(int[][] grid)
    {
        var working = CloneGrid(grid);
        var rows = working.Length;
        var cols = working[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (working[r][c] == 1)
                {
                    best = Math.Max(best, Flood(working, r, c, rows, cols));
                }
            }
        }

        return best;
    }

    private static int Flood(int[][] grid, int row, int col, int rows, int cols)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols || grid[row][col] != 1)
        {
            return 0;
        }

        grid[row][col] = 0;

        return 1
            + Flood(grid, row + 1, col, rows, cols)
            + Flood(grid, row - 1, col, rows, cols)
            + Flood(grid, row, col + 1, rows, cols)
            + Flood(grid, row, col - 1, rows, cols);
    }

    // This repo's own DFS: DepthFirstSearch.Traverse walks one island's full land
    // component from each unvisited land cell, and the traversal's own reachable-
    // node count (instead of a separate reachability set) is the island's area -
    // the same border-flood-fill primitive PacificAtlanticWaterFlow/SurroundedRegions
    // already use for LC 417/130.
    public static int MaxAreaByDepthFirstSearch(int[][] grid)
    {
        var working = CloneGrid(grid);
        var rows = working.Length;
        var cols = working[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (working[r][c] != 1)
                {
                    continue;
                }

                var island = DepthFirstSearch.Traverse((r, c), p => LandNeighbors(working, p));

                foreach (var (row, col) in island)
                {
                    working[row][col] = 0;
                }

                best = Math.Max(best, island.Count);
            }
        }

        return best;
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

        return grid[next.Row][next.Col] == 1;
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
