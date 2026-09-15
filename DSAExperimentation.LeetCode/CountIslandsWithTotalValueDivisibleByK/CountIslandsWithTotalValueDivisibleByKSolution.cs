using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.CountIslandsWithTotalValueDivisibleByK;

// LeetCode 3619. Count Islands With Total Value Divisible by K: flood-fill every
// 4-directionally connected group of positive cells, sum its values, and count
// the groups whose sum is a multiple of k.
internal static class CountIslandsWithTotalValueDivisibleByKSolution
{
    private static readonly (int DeltaRow, int DeltaCol)[] Orthogonal = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The textbook flood fill: a hand-rolled BCL Stack<T> and a visited bool[,],
    // walking one island at a time - the arm the DFS-engine strategy below has to
    // justify itself against.
    public static int CountByFloodFillStack(int[][] grid, int k)
    {
        var rows = grid.Length;
        var visited = new bool[rows, grid[0].Length];
        var islands = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                if (grid[row][col] <= 0 || visited[row, col])
                {
                    continue;
                }

                if (FloodFillTotal(grid, visited, row, col) % k == 0)
                {
                    islands++;
                }
            }
        }

        return islands;
    }

    private static long FloodFillTotal(int[][] grid, bool[,] visited, int startRow, int startCol)
    {
        var pending = new Stack<(int Row, int Col)>();
        pending.Push((startRow, startCol));
        visited[startRow, startCol] = true;
        var total = 0L;

        while (pending.TryPop(out var cell))
        {
            total += grid[cell.Row][cell.Col];
            PushUnvisitedNeighbors(grid, visited, pending, cell);
        }

        return total;
    }

    // Claim every orthogonal land neighbor of `cell` that no island has claimed yet,
    // so the flood fill reaches it in its own time.
    private static void PushUnvisitedNeighbors(
        int[][] grid, bool[,] visited, Stack<(int Row, int Col)> pending, (int Row, int Col) cell)
    {
        foreach (var (deltaRow, deltaCol) in Orthogonal)
        {
            var nextRow = cell.Row + deltaRow;
            var nextCol = cell.Col + deltaCol;

            if (IsUnvisitedLand(grid, visited, nextRow, nextCol))
            {
                visited[nextRow, nextCol] = true;
                pending.Push((nextRow, nextCol));
            }
        }
    }

    private static bool IsUnvisitedLand(int[][] grid, bool[,] visited, int row, int col)
        => row >= 0 && row < grid.Length && col >= 0 && col < grid[row].Length &&
           !visited[row, col] && grid[row][col] > 0;

    // DepthFirstSearch.Traverse (Algorithms.Traversal.DepthFirst) is already
    // "collect every node reachable from a root via an arbitrary successor
    // function" - a land cell's 4 orthogonal land neighbors is exactly that
    // relation, so the whole per-island collection step is one call; this
    // strategy only supplies the successor function and sums the returned cells.
    // Grid/GridTopology (DataStructures.Graph.Grids) isn't the fit here instead:
    // its nodes carry only passability, not a cell value, and IGraphTopology's
    // GetChildren is static-abstract so it cannot close over the runtime grid to
    // read one - exactly the "arbitrary successor relation" case
    // DepthFirstSearch's own doc comment carves out a Func-based engine for.
    public static int CountByDepthFirstSearchTraverse(int[][] grid, int k)
    {
        var rows = grid.Length;
        var visited = new HashSet<(int Row, int Col)>();
        var islands = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                if (grid[row][col] <= 0 || visited.Contains((row, col)))
                {
                    continue;
                }

                if (IsDivisibleIsland(grid, visited, (row, col), k))
                {
                    islands++;
                }
            }
        }

        return islands;
    }

    // The island at `start`: every land cell the traversal engine reaches from it,
    // claimed in one go, and whether the island's total value divides by k.
    private static bool IsDivisibleIsland(
        int[][] grid, HashSet<(int Row, int Col)> visited, (int Row, int Col) start, int k)
    {
        var island = DepthFirstSearch.Traverse<(int Row, int Col)>(start, cell => LandNeighbors(grid, cell));
        visited.UnionWith(island);

        return island.Sum(cell => (long)grid[cell.Row][cell.Col]) % k == 0;
    }

    private static IEnumerable<(int Row, int Col)> LandNeighbors(int[][] grid, (int Row, int Col) cell)
    {
        foreach (var (deltaRow, deltaCol) in Orthogonal)
        {
            var row = cell.Row + deltaRow;
            var col = cell.Col + deltaCol;

            if (IsLandOnBoard(grid, row, col))
            {
                yield return (row, col);
            }
        }
    }

    // A cell of the island worth walking to: on the grid, and positive land rather
    // than a zero or negative gap.
    private static bool IsLandOnBoard(int[][] grid, int row, int col)
        => row >= 0 && row < grid.Length && col >= 0 && col < grid[row].Length && grid[row][col] > 0;
}
