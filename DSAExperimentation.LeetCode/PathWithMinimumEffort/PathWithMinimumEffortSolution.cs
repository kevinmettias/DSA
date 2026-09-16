using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.PathWithMinimumEffort;

// LeetCode 1631. Path With Minimum Effort: walk from the top-left cell to the
// bottom-right one, minimizing the largest absolute height difference crossed
// on the way - a minimax (bottleneck) path, not a summed one.
//
// MinimumEffortPathByHeapDijkstra composes this repo's own Heap<Element,TOrder>
// ordered by ByPriorityOrder<TNode,TWeight>, the same frontier
// SwimInRisingWaterSolution.MinTimeByHeapDijkstra uses, with Math.Max standing
// in for +. The difference from LC 778 matters: Swim in Rising Water's cost is
// a property of the destination cell alone, so a cell can be marked visited the
// moment it is pushed. Here the edge weight depends on BOTH endpoints, so a
// cheaper route can still reach an already-pushed cell later - settling happens
// at pop time instead ("first pop is final", the rule ShortestPath.cs's own
// Settled set relies on), with a bestEffort table so a cell is only re-pushed
// for a strictly smaller candidate.
//
// MinimumEffortPathByBinarySearchFloodFill is the textbook alternative: binary
// search the answer, re-scanning the whole grid with a fresh BFS per candidate.
internal static class PathWithMinimumEffortSolution
{

    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // Deliberately written without this repo's primitives - a plain BCL
    // Queue<(int,int)> reachability scan per candidate effort is what you would
    // write without this repo. The search range is bounded by the grid's own
    // height spread, since no single step can cost more than that.
    public static int MinimumEffortPathByBinarySearchFloodFill(int[][] heights)
    {
        var grid = EffortGrid.Over(heights);
        var lo = 0;
        var hi = HeightSpread(grid);

        while (lo < hi)
        {
            var mid = lo + ((hi - lo) / AlgorithmConstants.HalvingFactor);

            if (CanReachWithEffort(grid, mid))
            {
                hi = mid;
            }
            else
            {
                lo = mid + 1;
            }
        }

        return lo;
    }

    private static int HeightSpread(EffortGrid grid)
    {
        var lowest = grid.Heights[0][0];
        var highest = grid.Heights[0][0];

        for (var row = 0; row < grid.Rows; row++)
        {
            for (var col = 0; col < grid.Cols; col++)
            {
                lowest = Math.Min(lowest, grid.Heights[row][col]);
                highest = Math.Max(highest, grid.Heights[row][col]);
            }
        }

        return highest - lowest;
    }

    private static bool CanReachWithEffort(EffortGrid grid, int effort)
    {
        var reach = new ReachWalk(new bool[grid.Rows, grid.Cols], new Queue<(int Row, int Col)>());
        reach.Queue.Enqueue((0, 0));
        reach.Visited[0, 0] = true;

        while (reach.Queue.Count > 0)
        {
            var current = reach.Queue.Dequeue();

            if (current.Row == grid.Rows - 1 && current.Col == grid.Cols - 1)
            {
                return true;
            }

            EnqueueReachableNeighbors(grid, effort, current, reach);
        }

        return false;
    }

    private static void EnqueueReachableNeighbors(
        EffortGrid grid, int effort, (int Row, int Col) current, ReachWalk reach)
    {
        foreach (var (dr, dc) in Directions)
        {
            var nr = current.Row + dr;
            var nc = current.Col + dc;

            if (IsOffGridOrAlreadyReached(grid, reach, nr, nc)
                || IsTooSteepToStepInto(grid, current, (nr, nc), effort))
            {
                continue;
            }

            reach.Visited[nr, nc] = true;
            reach.Queue.Enqueue((nr, nc));
        }
    }

    // Off the grid is not a cell at all, and one already reached has nothing new to offer.
    private static bool IsOffGridOrAlreadyReached(EffortGrid grid, ReachWalk reach, int row, int col) =>
        !grid.HasCell(row, col) || reach.Visited[row, col];

    // The step is only worth taking while the height change it costs stays inside the
    // effort the search is currently testing.
    private static bool IsTooSteepToStepInto(
        EffortGrid grid, (int Row, int Col) from, (int Row, int Col) to, int effort) =>
        Math.Abs(grid.Heights[to.Row][to.Col] - grid.Heights[from.Row][from.Col]) > effort;

    // One minimax-Dijkstra pass: each frontier entry carries the bottleneck
    // effort of the route that reached it, and popping the globally cheapest
    // entry first guarantees the first pop of a cell is its true minimum.
    public static int MinimumEffortPathByHeapDijkstra(int[][] heights)
    {
        var grid = EffortGrid.Over(heights);
        var walk = EffortWalk.Over(grid);

        walk.BestEffort[0, 0] = 0;
        walk.Frontier.Push(((0, 0), 0));

        while (walk.Frontier.TryPop(out var entry))
        {
            var settled = SettleEntry(grid, walk, entry);

            if (settled is { } effort)
            {
                return effort;
            }
        }

        // A 1x1 grid never enters the loop body's destination branch with a
        // neighbour left to relax; every larger grid is orthogonally connected,
        // so the destination is always popped before the frontier empties.
        return 0;
    }

    private static int? SettleEntry(EffortGrid grid, EffortWalk walk, ((int Row, int Col) Node, int Priority) entry)
    {
        var (row, col) = entry.Node;

        if (walk.Settled[row, col])
        {
            return null;
        }

        walk.Settled[row, col] = true;
        var effort = entry.Priority;

        if (row == grid.Rows - 1 && col == grid.Cols - 1)
        {
            return effort;
        }

        RelaxNeighbors(grid, walk, new SettledCell(row, col, effort));
        return null;
    }

    private static void RelaxNeighbors(EffortGrid grid, EffortWalk walk, SettledCell current)
    {
        foreach (var direction in Directions)
        {
            RelaxNeighbor(grid, walk, current, direction);
        }
    }

    private static void RelaxNeighbor(
        EffortGrid grid, EffortWalk walk, SettledCell current, (int Row, int Col) direction)
    {
        var (nr, nc) = NeighborOf(current, direction);

        if (!grid.HasCell(nr, nc) || walk.Settled[nr, nc])
        {
            return;
        }

        var candidate = Math.Max(
            current.Effort, Math.Abs(grid.Heights[nr][nc] - grid.Heights[current.Row][current.Col]));

        if (candidate < walk.BestEffort[nr, nc])
        {
            walk.BestEffort[nr, nc] = candidate;
            walk.Frontier.Push(((nr, nc), candidate));
        }
    }

    // The cell one step from `current` along `direction`, named before it is known to
    // be on the grid at all.
    private static (int Row, int Col) NeighborOf(SettledCell current, (int Row, int Col) direction) =>
        (current.Row + direction.Row, current.Col + direction.Col);

    private readonly record struct EffortGrid(int[][] Heights, int Rows, int Cols)
    {
        public static EffortGrid Over(int[][] heights) => new(heights, heights.Length, heights[0].Length);

        public bool HasCell(int row, int col) => row >= 0 && row < Rows && col >= 0 && col < Cols;
    }

    // A cell just settled by the Dijkstra pass, carrying the bottleneck effort
    // of the route that reached it so its neighbours can be relaxed from there.
    private readonly record struct SettledCell(int Row, int Col, int Effort);

    // The visited set and frontier of one flood-fill reachability scan.
    private readonly record struct ReachWalk(bool[,] Visited, Queue<(int Row, int Col)> Queue);

    private readonly record struct EffortWalk(
        bool[,] Settled,
        int[,] BestEffort,
        Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> Frontier)
    {
        public static EffortWalk Over(EffortGrid grid) =>
            new(new bool[grid.Rows, grid.Cols], UnreachedEfforts(grid), new());

        private static int[,] UnreachedEfforts(EffortGrid grid)
        {
            var bestEffort = new int[grid.Rows, grid.Cols];

            for (var row = 0; row < grid.Rows; row++)
            {
                for (var col = 0; col < grid.Cols; col++)
                {
                    bestEffort[row, col] = int.MaxValue;
                }
            }

            return bestEffort;
        }
    }
}
