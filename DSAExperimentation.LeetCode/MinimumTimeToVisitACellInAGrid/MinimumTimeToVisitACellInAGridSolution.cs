using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.MinimumTimeToVisitACellInAGrid;

// LeetCode 2577. Minimum Time to Visit a Cell In a Grid: grid[row][col] is the
// earliest second that cell may be entered; moving to an orthogonal neighbor always
// costs exactly one second, and arriving too early is never fatal - you can "wait" by
// bouncing back and forth with an already-visited neighbor two seconds at a time. This
// is still a non-negative-weight shortest-path relaxation (settle each cell once, at
// its true minimum arrival time), just with a per-edge cost that depends on the
// caller's current time rather than a static per-edge weight - one degree more dynamic
// than ShortestPath.Dijkstra's IEdgeTopology (a fixed weight per edge) can express, so
// this composes Collections.Heap<Element,TOrder> and Graph.ShortestPaths'
// ByPriorityOrder directly, the same frontier MinimumCostToMakeAtLeastOneValidPathInA
// GridTests already builds a bespoke relaxation loop over for the same reason. Both
// arms below run the identical algorithm; only the frontier's own type differs (BCL
// PriorityQueue vs. this repo's Heap), so the benchmark measures that one primitive
// swap in isolation (TwoSumBenchmarks/MinimumCostToMakeAtLeastOneValidPathInAGrid
// Benchmarks precedent - §17.5's "baseline stays BCL internally").
internal static class MinimumTimeToVisitACellInAGridSolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];

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

        if (!AnyFirstMoveIsAvailable(grid, rows, cols))
        {
            return -1;
        }

        var distances = new Dictionary<(int Row, int Col), int> { [(0, 0)] = 0 };
        var settled = new HashSet<(int Row, int Col)>();
        var frontier = new PriorityQueue<(int Row, int Col), int>();
        frontier.Enqueue((0, 0), 0);

        while (frontier.TryDequeue(out var node, out var priority))
        {
            if (!settled.Add(node))
            {
                continue;
            }

            if (node.Row == rows - 1 && node.Col == cols - 1)
            {
                return priority;
            }

            foreach (var neighbor in Neighbors(node, rows, cols))
            {
                var arrival = ArrivalTime(priority, grid[neighbor.Row][neighbor.Col]);

                if (!distances.TryGetValue(neighbor, out var known) || arrival < known)
                {
                    distances[neighbor] = arrival;
                    frontier.Enqueue(neighbor, arrival);
                }
            }
        }

        return -1;
    }

    // Composed: identical algorithm, fronted by Collections.Heap<Element,TOrder>
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

        if (!AnyFirstMoveIsAvailable(grid, rows, cols))
        {
            return -1;
        }

        var distances = new Dictionary<(int Row, int Col), int> { [(0, 0)] = 0 };
        var settled = new HashSet<(int Row, int Col)>();
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();
        frontier.Push(((0, 0), 0));

        while (frontier.TryPop(out var entry))
        {
            if (!settled.Add(entry.Node))
            {
                continue;
            }

            if (entry.Node.Row == rows - 1 && entry.Node.Col == cols - 1)
            {
                return entry.Priority;
            }

            foreach (var neighbor in Neighbors(entry.Node, rows, cols))
            {
                var arrival = ArrivalTime(entry.Priority, grid[neighbor.Row][neighbor.Col]);

                if (!distances.TryGetValue(neighbor, out var known) || arrival < known)
                {
                    distances[neighbor] = arrival;
                    frontier.Push((neighbor, arrival));
                }
            }
        }

        return -1;
    }

    // The only move out of (0,0) at second 0 with no predecessor yet to bounce with -
    // every later step always has one (the cell it just arrived from), but the very
    // first step doesn't, so it's the one place a cell requiring second > 1 can make
    // the grid genuinely unsolvable rather than just "worth waiting for".
    private static bool AnyFirstMoveIsAvailable(int[][] grid, int rows, int cols)
    {
        var canGoRight = cols > 1 && grid[0][1] <= 1;
        var canGoDown = rows > 1 && grid[1][0] <= 1;
        return canGoRight || canGoDown;
    }

    private static IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) node, int rows, int cols)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: node.Row + dRow, Col: node.Col + dCol);

            if (next.Row >= 0 && next.Row < rows && next.Col >= 0 && next.Col < cols)
            {
                yield return next;
            }
        }
    }

    // Earliest arrival at a cell requiring `requiredTime` seconds, moving on from
    // `currentTime`: one second to step in, then - if that's still too early - an even
    // number of extra seconds bouncing with the predecessor, since the grid is
    // bipartite by (row + col) parity and every second flips it, so an odd shortfall
    // needs one more second than the shortfall itself to land back on a valid parity.
    private static int ArrivalTime(int currentTime, int requiredTime)
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
