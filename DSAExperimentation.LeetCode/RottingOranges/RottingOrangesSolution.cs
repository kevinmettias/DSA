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
    // The problem's own cell encoding: 0 empty, 1 fresh, 2 rotten.
    private const int FreshOrangeState = 1;

    private const int RottenOrangeState = 2;
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

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
        var (nextRow, nextCol) = NeighborCell(current, direction);
        rottenMinutes = LeetCodeAnswer.None;

        if (IsOutOfBounds(state.Grid, nextRow, nextCol) || state.Visited[nextRow, nextCol])
        {
            return false;
        }

        return TryConsumeNeighbor(state, (nextRow, nextCol), current, out rottenMinutes);
    }

    // The cell one step from current along direction.
    private static (int Row, int Col) NeighborCell(
        (int Row, int Col, int Minutes) current, (int DRow, int DCol) direction)
        => (current.Row + direction.DRow, current.Col + direction.DCol);

    // Consumes one on-grid, unvisited neighbor of the dequeued cell: a rotten one ends
    // the search with the distance to it, a fresh one joins the frontier at the next
    // minute, and anything else is left alone.
    private static bool TryConsumeNeighbor(
        SingleSourceBfsState state,
        (int Row, int Col) cell,
        (int Row, int Col, int Minutes) current,
        out int rottenMinutes)
    {
        if (state.Grid[cell.Row][cell.Col] == RottenOrangeState)
        {
            rottenMinutes = current.Minutes + 1;
            return true;
        }

        if (state.Grid[cell.Row][cell.Col] == FreshOrangeState)
        {
            state.Visited[cell.Row, cell.Col] = true;
            state.Queue.Enqueue((cell.Row, cell.Col, current.Minutes + 1));
        }

        rottenMinutes = LeetCodeAnswer.None;
        return false;
    }

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
        var rotGrid = new RotGrid(grid, visited, minutesToRot, frontier);

        ScanGridForSeeds(rotGrid, ref progress);

        return (rotGrid, progress);
    }

    // Allocates each row of minutesToRot, then enqueues every already-rotten cell as
    // a minute-0 BFS seed and counts the fresh ones. The self-contained scan step of
    // the initialization above - it takes the very RotGrid it is filling, which is
    // exactly the four structures the scan threads, so they travel as one value.
    private static void ScanGridForSeeds(RotGrid grid, ref BfsProgress progress)
    {
        for (var row = 0; row < grid.Grid.Length; row++)
        {
            grid.MinutesToRot[row] = new int[grid.Grid[row].Length];

            for (var col = 0; col < grid.Grid[row].Length; col++)
            {
                if (grid.Grid[row][col] == RottenOrangeState)
                {
                    grid.Frontier.Enqueue((row, col));
                    grid.Visited[row, col] = true;
                }
                else if (grid.Grid[row][col] == FreshOrangeState)
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

            if (IsOutOfBounds(grid.Grid, nextRow, nextCol) || IsUnvisitedFreshOrange(grid, nextRow, nextCol))
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

    // A neighbor this sweep has not reached yet that still has a fresh orange on it -
    // the only cells the multi-source BFS expands into.
    private static bool IsUnvisitedFreshOrange(RotGrid grid, int row, int col)
        => !grid.Visited[row, col] && grid.Grid[row][col] == FreshOrangeState;

    private static bool IsOutOfBounds(int[][] grid, int row, int col) =>
        row < 0 || row >= grid.Length || col < 0 || col >= grid[row].Length;

    // The grid, visited map and frontier queue a single-source BFS expands into -
    // bundled so TryVisitNeighbor stays within the parameter-count limit.
    private readonly record struct SingleSourceBfsState(
        int[][] Grid,
        bool[,] Visited,
        System.Collections.Generic.Queue<(int Row, int Col, int Minutes)> Queue);

    // The read-only grid plus the three shared, in-place-mutated structures the
    // multi-source BFS expands into - bundled so ExpandRottenCell stays within the
    // parameter-count limit.
    private readonly record struct RotGrid(
        int[][] Grid,
        bool[,] Visited,
        int[][] MinutesToRot,
        RepoQueue Frontier);

    // The two running totals ExpandRottenCell updates on every reachable neighbor.
    private sealed class BfsProgress
    {
        public int FreshCount { get; set; }
        public int MinutesElapsed { get; set; }
    }
}
