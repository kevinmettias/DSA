namespace DSAExperimentation.LeetCode.MinimumTimeToVisitACellInAGrid;

// LeetCode 2577. Minimum Time to Visit a Cell In a Grid: grid[row][col] is the
// earliest second that cell may be entered; moving to an orthogonal neighbor always
// costs exactly one second, and arriving too early is never fatal - you can "wait" by
// bouncing back and forth with an already-visited neighbor two seconds at a time. This
// is still a non-negative-weight shortest-path relaxation (settle each cell once, at
// its true minimum arrival time), just with a per-edge cost that depends on the
// caller's current time rather than a static per-edge weight - one degree more dynamic
// than ShortestPath.Dijkstra's IEdgeTopology (a fixed weight per edge) can express. The
// relaxation itself is therefore GridArrivalDijkstra's, declared in this folder because
// this class is the one that states what makes a grid search dynamic; only the arrival rule
// below - ParityBounceArrivalRule, the parity wait this problem's bounce forces - and the
// "is there any first move at all" precondition are this problem's own, since LC 3341's grid
// has neither a bounce nor an unsolvable opening.
// Both arms below run the identical algorithm; only the frontier's own type differs (BCL
// PriorityQueue vs. this repo's Heap), so the benchmark measures that one primitive
// swap in isolation (TwoSumBenchmarks/MinimumCostToMakeAtLeastOneValidPathInAGrid
// Benchmarks precedent - §17.5's "baseline stays BCL internally").
internal static class MinimumTimeToVisitACellInAGridSolution
{
    // Baseline: the same dynamic-wait relaxation, fronted by the BCL's own
    // PriorityQueue<TElement,TPriority> instead of this repo's Heap - "what you'd
    // write without this repo" (§17.5).
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

        return GridArrivalDijkstra.ByBclQueue(grid, new ParityBounceArrivalRule());
    }

    // Composed: identical algorithm, fronted by this repo's own Heap<Element,TOrder>
    // ordered by ByPriorityOrder<TNode,TWeight> - the same production heap
    // ShortestPath.Dijkstra/AStar and every other grid-Dijkstra coverage test in this
    // repo already use as their frontier.
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

        return GridArrivalDijkstra.ByHeap(grid, new ParityBounceArrivalRule());
    }

    // The only move out of (0,0) at second 0 with no predecessor yet to bounce with -
    // every later step always has one (the cell it just arrived from), but the very
    // first step doesn't, so it's the one place a cell requiring second > 1 can make
    // the grid genuinely unsolvable rather than just "worth waiting for".
    private static bool CanMakeAnyFirstMove(int[][] grid, int rows, int cols)
    {
        var canGoRight = cols > 1 && grid[0][1] <= 1;
        var canGoDown = rows > 1 && grid[1][0] <= 1;
        return canGoRight || canGoDown;
    }

    // Earliest arrival at a cell requiring `requiredTime` seconds, moving on from
    // `currentTime`: one second to step in, then - if that's still too early - an even
    // number of extra seconds bouncing with the predecessor, since the grid is
    // bipartite by (row + col) parity and every second flips it, so an odd shortfall
    // needs one more second than the shortfall itself to land back on a valid parity.
    //
    // This is the whole of what LC 2577 asks of the shared relaxation: everything else
    // GridArrivalDijkstra already knows, so the rule is an IArrivalRule implementation and
    // no other part of this problem's answer needs to live outside the two arms above.
    private sealed class ParityBounceArrivalRule : IArrivalRule
    {
        public int Arrive(int currentTime, int requiredTime)
        {
            var earliest = currentTime + 1;

            if (earliest >= requiredTime)
            {
                return earliest;
            }

            var wait = requiredTime - earliest;

            if (wait % 2 != 0)
            {
                wait++;
            }

            return earliest + wait;
        }
    }
}
