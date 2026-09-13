using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>;

namespace DSAExperimentation.LeetCode.RottingOranges;

// LeetCode 994. Rotting Oranges: every minute, each fresh orange 4-directionally
// adjacent to a rotten one turns rotten; report the minute at which no fresh orange
// is left, or -1 if one can never be reached.
//
// The minute a fresh cell rots is its BFS distance, through fresh cells only, to the
// nearest initially-rotten cell. The textbook baseline therefore runs one
// independent single-source BFS per fresh orange and takes the maximum; the composed
// strategy seeds one shared frontier - this repo's own Queue<TElement> - with every
// already-rotten orange at minute 0, so a single multi-source sweep discovers each
// cell exactly once via its true nearest source. The same shape ZeroOneMatrix uses
// for LC 542, tracking minutes-to-rot instead of distance-to-nearest-zero.
internal static class RottingOrangesSolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The problem's own cell encoding: 0 empty, 1 fresh, 2 rotten.
    private const int FreshOrangeState = 1;
    private const int RottenOrangeState = 2;

    // The naive baseline: a freshly allocated visited grid and BCL Queue per fresh
    // orange, re-walked outward until it reaches a rotten one. Deliberately written
    // without this repo's primitives - it is the arm the composed solution has to
    // justify itself against.
    public static int OrangesRottingByPerCellBfs(int[][] grid)
    {
        var maxMinutes = 0;

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                if (!TryAccumulateFreshCellMinutes(grid, row, col, ref maxMinutes))
                {
                    return LeetCodeAnswer.None;
                }
            }
        }

        return maxMinutes;
    }

    // Handles one grid cell for the baseline: skips non-fresh cells, otherwise folds
    // its nearest-rotten distance into maxMinutes. Returns false to signal that no
    // rotten orange can reach this cell, so the caller must abort with -1.
    private static bool TryAccumulateFreshCellMinutes(int[][] grid, int row, int col, ref int maxMinutes)
    {
        if (grid[row][col] != FreshOrangeState)
        {
            return true;
        }

        var minutes = NearestRottenDistance(grid, row, col);

        if (minutes < 0)
        {
            return false;
        }

        maxMinutes = Math.Max(maxMinutes, minutes);
        return true;
    }

    private static int NearestRottenDistance(int[][] grid, int startRow, int startCol)
    {
        var visited = new bool[grid.Length, grid[0].Length];
        var queue = new System.Collections.Generic.Queue<(int Row, int Col, int Minutes)>();
        visited[startRow, startCol] = true;
        queue.Enqueue((startRow, startCol, 0));

        var state = new SingleSourceBfsState(grid, visited, queue);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (TryExpandCell(current, state, out var rottenMinutes))
            {
                return rottenMinutes;
            }
        }

        return LeetCodeAnswer.None;
    }

    // Visits the four neighbors of the dequeued cell: if one is rotten, reports the
    // distance to it; otherwise enqueues every unvisited fresh neighbor for later
    // expansion.
    private static bool TryExpandCell(
        (int Row, int Col, int Minutes) current, SingleSourceBfsState state, out int rottenMinutes)
    {
        foreach (var direction in Directions)
        {
            if (TryVisitNeighbor(current, direction, state, out var foundMinutes))
            {
                rottenMinutes = foundMinutes;
                return true;
            }
        }

        rottenMinutes = LeetCodeAnswer.None;
        return false;
    }

    // Checks a single neighbor in the given direction: reports the distance if it's
    // rotten, enqueues it if it's an unvisited fresh cell, otherwise does nothing.
    // The self-contained per-neighbor step of the BFS above.
    private static bool TryVisitNeighbor(
        (int Row, int Col, int Minutes) current,
        (int DRow, int DCol) direction,
        SingleSourceBfsState state,
        out int rottenMinutes)
    {
        var nextRow = current.Row + direction.DRow;
        var nextCol = current.Col + direction.DCol;
        rottenMinutes = LeetCodeAnswer.None;

        if (IsOutOfBounds(state.Grid, nextRow, nextCol) || state.Visited[nextRow, nextCol])
        {
            return false;
        }

        if (state.Grid[nextRow][nextCol] == RottenOrangeState)
        {
            rottenMinutes = current.Minutes + 1;
            return true;
        }

        if (state.Grid[nextRow][nextCol] == FreshOrangeState)
        {
            state.Visited[nextRow, nextCol] = true;
            state.Queue.Enqueue((nextRow, nextCol, current.Minutes + 1));
        }

        return false;
    }

    // The grid, visited map and frontier queue a single-source BFS expands into -
    // bundled so TryVisitNeighbor stays within the parameter-count limit.
    private readonly record struct SingleSourceBfsState(
        int[][] Grid,
        bool[,] Visited,
        System.Collections.Generic.Queue<(int Row, int Col, int Minutes)> Queue);

    // This repo's own multi-source BFS: seed the frontier with every already-rotten
    // orange at minute 0 simultaneously, using Queue<TElement> as the FIFO frontier,
    // so the first time a fresh cell is reached is necessarily at its true rotting
    // minute. The input grid is left untouched; rot state lives in the visited map.
    public static int OrangesRottingByMultiSourceBfs(int[][] grid)
    {
        var (rotGrid, progress) = InitializeRotGrid(grid);

        Drain(rotGrid, ref progress);

        return progress.FreshCount == 0 ? progress.MinutesElapsed : LeetCodeAnswer.None;
    }

    // Allocates the per-cell arrays, then scans the grid once to seed the BFS
    // frontier with every already-rotten cell and count the fresh ones.
    private static (RotGrid Grid, BfsProgress Progress) InitializeRotGrid(int[][] grid)
    {
        var minutesToRot = new int[grid.Length][];
        var visited = new bool[grid.Length, grid[0].Length];
        var frontier = new RepoQueue();
        var progress = new BfsProgress();

        ScanGridForSeeds(grid, minutesToRot, visited, frontier, ref progress);

        return (new RotGrid(grid, visited, minutesToRot, frontier), progress);
    }

    // Allocates each row of minutesToRot, then enqueues every already-rotten cell as
    // a minute-0 BFS seed and counts the fresh ones. The self-contained scan step of
    // the initialization above.
    private static void ScanGridForSeeds(
        int[][] grid,
        int[][] minutesToRot,
        bool[,] visited,
        RepoQueue frontier,
        ref BfsProgress progress)
    {
        for (var row = 0; row < grid.Length; row++)
        {
            minutesToRot[row] = new int[grid[row].Length];

            for (var col = 0; col < grid[row].Length; col++)
            {
                if (grid[row][col] == RottenOrangeState)
                {
                    frontier.Enqueue((row, col));
                    visited[row, col] = true;
                }
                else if (grid[row][col] == FreshOrangeState)
                {
                    progress.FreshCount++;
                }
            }
        }
    }

    // Drains the frontier, expanding one rotten cell at a time until none remain.
    private static void Drain(RotGrid grid, ref BfsProgress progress)
    {
        while (grid.Frontier.TryDequeue(out var cell))
        {
            ExpandRottenCell(cell, grid, ref progress);
        }
    }

    // Visits the four neighbors of a just-dequeued rotten/rotting cell: marks each
    // reachable fresh neighbor rotten, records its minute, and enqueues it. The
    // self-contained per-cell step of the multi-source BFS above.
    private static void ExpandRottenCell((int Row, int Col) cell, RotGrid grid, ref BfsProgress progress)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = cell.Row + dRow;
            var nextCol = cell.Col + dCol;

            if (IsOutOfBounds(grid.Grid, nextRow, nextCol) || grid.Visited[nextRow, nextCol]
                || grid.Grid[nextRow][nextCol] != FreshOrangeState)
            {
                continue;
            }

            grid.Visited[nextRow, nextCol] = true;
            grid.MinutesToRot[nextRow][nextCol] = grid.MinutesToRot[cell.Row][cell.Col] + 1;
            progress.FreshCount--;
            progress.MinutesElapsed = Math.Max(progress.MinutesElapsed, grid.MinutesToRot[nextRow][nextCol]);
            grid.Frontier.Enqueue((nextRow, nextCol));
        }
    }

    private static bool IsOutOfBounds(int[][] grid, int row, int col) =>
        row < 0 || row >= grid.Length || col < 0 || col >= grid[row].Length;

    // The read-only grid plus the three shared, in-place-mutated structures the
    // multi-source BFS expands into - bundled so ExpandRottenCell stays within the
    // parameter-count limit.
    private readonly record struct RotGrid(
        int[][] Grid,
        bool[,] Visited,
        int[][] MinutesToRot,
        RepoQueue Frontier);

    // The two running totals ExpandRottenCell updates on every reachable neighbor.
    private struct BfsProgress
    {
        public int FreshCount;
        public int MinutesElapsed;
    }
}
