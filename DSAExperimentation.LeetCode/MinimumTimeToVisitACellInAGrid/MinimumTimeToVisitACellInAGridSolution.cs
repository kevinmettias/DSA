using DSAExperimentation.Algorithms.ShortestPaths.Grids;

namespace DSAExperimentation.LeetCode.MinimumTimeToVisitACellInAGrid;

// LeetCode 2577. Minimum Time to Visit a Cell In a Grid: grid[row][col] is the
// earliest second that cell may be entered; moving to an orthogonal neighbor always
// costs exactly one second, and arriving too early is never fatal - you can "wait" by
// bouncing back and forth with an already-visited neighbor two seconds at a time. This
// is still a non-negative-weight shortest-path relaxation (settle each cell once, at
// its true minimum arrival time), just with a per-edge cost that depends on the
// caller's current time rather than a static per-edge weight - the time-dependent grid
// search Algorithms.ShortestPaths.Grids.GridEarliestArrival runs, priced here by
// ParityBounceArrival. The one thing that rule cannot know is this problem's opening:
// the very first move has no cell behind it to bounce with, so a start whose
// neighbours all open after second 1 makes the grid unsolvable rather than just slow.
// Both arms check that first.
internal static class MinimumTimeToVisitACellInAGridSolution
{
    // The latest second the first move may land, there being nothing to bounce with yet.
    private const int LatestFirstArrival = 1;

    // There and back with the predecessor: the wait an early arrival can make.
    private const int SecondsPerBounce = 2;

    private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];

    // Baseline: the textbook Dijkstra over the grid - a BCL PriorityQueue frontier, plain
    // arrays for each cell's best arrival and whether it has settled, and the bounce wait
    // written inline - "what you'd write without this repo" (ARCHITECTURE.md 17.5).
    public static int MinimumTimeByBclPriorityQueue(int[][] grid)
    {
        var (rows, cols) = (grid.Length, grid[0].Length);

        if (rows == 1 && cols == 1)
        {
            return 0;
        }

        if (!CanMakeAnyFirstMove(grid, rows, cols))
        {
            return LeetCodeAnswer.None;
        }

        return EarliestArrivalByBclQueue(grid);
    }

    // Composed: GridEarliestArrival, this repo's time-dependent grid search over its own
    // Heap, priced by ParityBounceArrival.
    public static int MinimumTimeByHeap(int[][] grid)
    {
        var (rows, cols) = (grid.Length, grid[0].Length);

        if (rows == 1 && cols == 1)
        {
            return 0;
        }

        if (!CanMakeAnyFirstMove(grid, rows, cols))
        {
            return LeetCodeAnswer.None;
        }

        return GridEarliestArrival.Time<ParityBounceArrival>(grid, (0, 0), (rows - 1, cols - 1));
    }

    // The only move out of (0,0) at second 0 with no predecessor yet to bounce with -
    // every later step always has one (the cell it just arrived from), but the very
    // first step doesn't, so it's the one place a cell requiring second > 1 can make
    // the grid genuinely unsolvable rather than just "worth waiting for".
    private static bool CanMakeAnyFirstMove(int[][] grid, int rows, int cols)
    {
        var canGoRight = cols > 1 && grid[0][1] <= LatestFirstArrival;
        var canGoDown = rows > 1 && grid[1][0] <= LatestFirstArrival;
        return canGoRight || canGoDown;
    }

    // Settle cells in nondecreasing arrival order until the bottom-right one comes off the
    // queue. A better arrival re-enters the queue rather than replacing the earlier entry,
    // and the stale entry is skipped when it surfaces because its cell has settled.
    private static int EarliestArrivalByBclQueue(int[][] grid)
    {
        var search = new BounceSearch(
            grid, Unreached(grid), new bool[grid.Length, grid[0].Length], new PriorityQueue<(int Row, int Col), int>());
        var lastCell = (grid.Length - 1, grid[0].Length - 1);

        search.Best[0, 0] = 0;
        search.Frontier.Enqueue((0, 0), 0);

        while (search.Frontier.TryDequeue(out var cell, out var time))
        {
            if (cell == lastCell)
            {
                return time;
            }

            if (!search.Settled[cell.Row, cell.Col])
            {
                search.Settled[cell.Row, cell.Col] = true;
                OfferNeighbours(search, cell, time);
            }
        }

        return LeetCodeAnswer.None;
    }

    // Every cell's best arrival before the search has reached it.
    private static int[,] Unreached(int[][] grid)
    {
        var best = new int[grid.Length, grid[0].Length];

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[0].Length; col++)
            {
                best[row, col] = int.MaxValue;
            }
        }

        return best;
    }

    // Every unsettled neighbour the move from `cell` reaches sooner than any route found so far.
    private static void OfferNeighbours(BounceSearch search, (int Row, int Col) cell, int time)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var (row, col) = (cell.Row + dRow, cell.Col + dCol);

            if (!IsUnsettledCell(search, row, col))
            {
                continue;
            }

            var arrival = BounceArrival(time, search.Grid[row][col]);

            if (arrival < search.Best[row, col])
            {
                search.Best[row, col] = arrival;
                search.Frontier.Enqueue((row, col), arrival);
            }
        }
    }

    // On the grid, and not yet settled.
    private static bool IsUnsettledCell(BounceSearch search, int row, int col) =>
        row >= 0 && row < search.Grid.Length && col >= 0 && col < search.Grid[0].Length && !search.Settled[row, col];

    // One second to step in, then - if that is still too early - an even number of extra
    // seconds bouncing with the predecessor, since every second flips the (row + col)
    // parity: an odd shortfall needs one more second than the shortfall itself.
    private static int BounceArrival(int departure, int enterableAt)
    {
        var earliest = departure + 1;

        if (earliest >= enterableAt)
        {
            return earliest;
        }

        var leftOverSecond = (enterableAt - earliest) % SecondsPerBounce;

        return enterableAt + leftOverSecond;
    }

    // One search's state: the grid, each cell's best arrival so far, which cells have
    // settled, and the queue of pending arrivals.
    private sealed record BounceSearch(
        int[][] Grid, int[,] Best, bool[,] Settled, PriorityQueue<(int Row, int Col), int> Frontier);
}
