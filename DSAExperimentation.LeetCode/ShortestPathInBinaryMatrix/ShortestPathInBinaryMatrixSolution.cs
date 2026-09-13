using BclQueue = System.Collections.Generic.Queue<(int Row, int Col)>;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>;

namespace DSAExperimentation.LeetCode.ShortestPathInBinaryMatrix;

// LeetCode 1091. Shortest Path in Binary Matrix: the number of cells on the
// shortest clear path from the top-left corner to the bottom-right corner of an
// n x n binary matrix, moving in any of the 8 king directions through 0 cells
// only, or -1 when no such path exists.
//
// Both strategies are the same single-source breadth-first sweep - the answer is
// the BFS distance to the far corner, counted in cells rather than steps, so the
// start cell sits at distance 1. They differ only in the FIFO frontier they drain:
// the baseline uses the BCL's Queue<T>, the composed strategy uses this repo's own
// Queue<TElement>, the same frontier RottingOranges and ZeroOneMatrix use for their
// 4-directional grid sweeps. Graph/Grids' GridChildren is hardcoded to the 4
// orthogonal neighbours and has no diagonal mode, so neither arm can go through
// that topology for this problem's 8 directions.
//
// A distance of 0 means "not yet reached": real distances start at 1 for the start
// cell, so the freshly allocated distance grid already reads as unvisited and needs
// no sentinel fill.
internal static class ShortestPathInBinaryMatrixSolution
{
    // All 8 king moves - diagonal steps are legal in this problem.
    private static readonly (int DRow, int DCol)[] Directions =
    [
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1),
    ];

    // The problem's own cell encoding: 0 is clear ground, 1 is blocked.
    private const int ClearCell = 0;

    // The textbook answer: a BCL Queue<T> as the frontier and a plain rectangular
    // int grid as the distance map, generating each of the 8 candidate moves on the
    // fly. Deliberately written without this repo's primitives - it is the arm the
    // composed strategy below has to justify itself against.
    public static int ShortestPathBinaryMatrixByBclQueue(int[][] grid)
    {
        if (IsEitherCornerBlocked(grid))
        {
            return LeetCodeAnswer.None;
        }

        var field = CreateSeededField(grid);
        var frontier = new BclQueue();
        frontier.Enqueue((0, 0));

        while (frontier.Count > 0)
        {
            var cell = frontier.Dequeue();

            if (IsFarCorner(field, cell))
            {
                return field.Distance[cell.Row, cell.Col];
            }

            foreach (var direction in Directions)
            {
                if (TryRelax(field, cell, direction, out var next))
                {
                    frontier.Enqueue(next);
                }
            }
        }

        return LeetCodeAnswer.None;
    }

    // The same sweep over this repo's own Queue<TElement>, whose TryDequeue drives
    // the loop directly instead of a separate Count check plus Dequeue.
    public static int ShortestPathBinaryMatrixByQueueFrontier(int[][] grid)
    {
        if (IsEitherCornerBlocked(grid))
        {
            return LeetCodeAnswer.None;
        }

        var field = CreateSeededField(grid);
        var frontier = new RepoQueue();
        frontier.Enqueue((0, 0));

        while (frontier.TryDequeue(out var cell))
        {
            if (IsFarCorner(field, cell))
            {
                return field.Distance[cell.Row, cell.Col];
            }

            foreach (var direction in Directions)
            {
                if (TryRelax(field, cell, direction, out var next))
                {
                    frontier.Enqueue(next);
                }
            }
        }

        return LeetCodeAnswer.None;
    }

    // LeetCode guarantees a square grid, so either corner being blocked settles the
    // answer before any search starts.
    private static bool IsEitherCornerBlocked(int[][] grid)
    {
        var n = grid.Length;

        return grid[0][0] != ClearCell || grid[n - 1][n - 1] != ClearCell;
    }

    // Allocates the distance map and marks the top-left cell as reached at
    // distance 1 - the cell count of a path that is just the start cell.
    private static PathField CreateSeededField(int[][] grid)
    {
        var distance = new int[grid.Length, grid.Length];
        distance[0, 0] = 1;

        return new PathField(grid, distance);
    }

    private static bool IsFarCorner(PathField field, (int Row, int Col) cell) =>
        cell.Row == field.Size - 1 && cell.Col == field.Size - 1;

    // Computes the neighbour one step from `cell` in `direction` and, if it is in
    // bounds, clear, and not yet reached, records its distance and reports it so the
    // caller can enqueue it. The self-contained relaxation step of both sweeps.
    private static bool TryRelax(
        PathField field, (int Row, int Col) cell, (int DRow, int DCol) direction, out (int Row, int Col) next)
    {
        var nextRow = cell.Row + direction.DRow;
        var nextCol = cell.Col + direction.DCol;
        next = default;

        if (nextRow < 0 || nextRow >= field.Size || nextCol < 0 || nextCol >= field.Size
            || field.Grid[nextRow][nextCol] != ClearCell || field.Distance[nextRow, nextCol] != 0)
        {
            return false;
        }

        field.Distance[nextRow, nextCol] = field.Distance[cell.Row, cell.Col] + 1;
        next = (nextRow, nextCol);
        return true;
    }

    // The read-only grid and the in-place-mutated distance map both sweeps relax
    // into - bundled so TryRelax reads one board rather than two loose arrays.
    private readonly record struct PathField(int[][] Grid, int[,] Distance)
    {
        public int Size => Grid.Length;
    }
}
