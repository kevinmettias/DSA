using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.CheckIfThereIsAValidPathInAGrid;

// LeetCode 1391. Check if There is a Valid Path in a Grid: each street piece (1-6)
// opens toward a fixed pair of the four directions, and a move from one cell to an
// orthogonal neighbor is only valid when both cells open toward each other. The
// question is whether (0,0) reaches the bottom-right corner under that relation.
//
// That "successor" rule is arbitrary per-cell logic rather than the fixed
// passable/impassable check GridTopology models, so this is an implicit graph -
// the same shape PacificAtlanticWaterFlowSolution works in. Both strategies below
// walk the identical street-compatibility rule, so the comparison isolates the
// traversal machinery (recursion + a bool[,] visited array vs. an explicit stack
// and a hash set) rather than the per-cell logic.
internal static class CheckIfThereIsAValidPathInAGridSolution
{
    // The problem's whole content: which two of the four directions each street
    // piece opens toward. Meaningless outside LC 1391, so it lives beside the
    // strategies rather than in Domain/ (ARCHITECTURE.md section 17.3).
    private static readonly Dictionary<int, (int DRow, int DCol)[]> Openings = new()
    {
        [1] = [(0, -1), (0, 1)],
        [2] = [(-1, 0), (1, 0)],
        [3] = [(0, -1), (1, 0)],
        [4] = [(0, 1), (1, 0)],
        [5] = [(0, -1), (-1, 0)],
        [6] = [(0, 1), (-1, 0)],
    };

    // The textbook answer: hand-rolled recursive depth-first search over a
    // rows*cols bool[,] visited array, returning as soon as the corner is reached.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed strategy below has to justify itself against.
    public static bool HasValidPathByRecursiveDfs(int[][] grid)
    {
        var visited = new bool[grid.Length, grid[0].Length];

        return Dfs((0, 0), visited, grid);
    }

    // This repo's own DepthFirstSearch.Traverse over a bespoke successor function -
    // an explicit stack plus a visited hash set, with no recursion depth bounded by
    // the grid's cell count - reporting whether the bottom-right corner shows up in
    // the reachable set from (0,0).
    public static bool HasValidPathByDepthFirstTraverse(int[][] grid)
    {
        var corner = (Row: grid.Length - 1, Col: grid[0].Length - 1);

        return DepthFirstSearch.Traverse((Row: 0, Col: 0), cell => Neighbors(cell, grid)).Contains(corner);
    }

    private static IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) cell, int[][] grid)
    {
        foreach (var direction in Openings[grid[cell.Row][cell.Col]])
        {
            var next = (Row: cell.Row + direction.DRow, Col: cell.Col + direction.DCol);

            if (OpensBackToward(next, direction, grid))
            {
                yield return next;
            }
        }
    }

    private static bool Dfs((int Row, int Col) cell, bool[,] visited, int[][] grid)
    {
        if (visited[cell.Row, cell.Col])
        {
            return false;
        }

        visited[cell.Row, cell.Col] = true;

        if (cell.Row == grid.Length - 1 && cell.Col == grid[0].Length - 1)
        {
            return true;
        }

        return TryReachCornerFromNeighbor(cell, visited, grid);
    }

    private static bool TryReachCornerFromNeighbor((int Row, int Col) cell, bool[,] visited, int[][] grid)
    {
        foreach (var direction in Openings[grid[cell.Row][cell.Col]])
        {
            if (TryReachCornerViaOpening(cell, direction, visited, grid))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryReachCornerViaOpening(
        (int Row, int Col) cell, (int DRow, int DCol) direction, bool[,] visited, int[][] grid)
    {
        var next = (Row: cell.Row + direction.DRow, Col: cell.Col + direction.DCol);

        if (!OpensBackToward(next, direction, grid) || visited[next.Row, next.Col])
        {
            return false;
        }

        return Dfs(next, visited, grid);
    }

    // The street-compatibility rule both strategies walk: the neighbor must be on
    // the board and must itself open back along the direction it was entered from.
    // Plain array indexing and Array.IndexOf, so sharing it leaves the baseline's
    // textbook character intact (ARCHITECTURE.md section 17.5).
    private static bool OpensBackToward((int Row, int Col) next, (int DRow, int DCol) direction, int[][] grid)
    {
        if (!IsOnBoard(next, grid))
        {
            return false;
        }

        return Array.IndexOf(Openings[grid[next.Row][next.Col]], (-direction.DRow, -direction.DCol)) >= 0;
    }

    // Whether the cell lies on the board at all.
    private static bool IsOnBoard((int Row, int Col) cell, int[][] grid)
        => cell.Row >= 0 && cell.Row < grid.Length && cell.Col >= 0 && cell.Col < grid[0].Length;
}
