using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.SwimInRisingWater;

// LeetCode 778. Swim in Rising Water: minimum time until (0,0) can reach
// (n-1,n-1), where a cell can only be entered once the rising water level
// reaches its own elevation - equivalently, the minimax (bottleneck) path cost
// from corner to corner over the grid's own elevations.
//
// MinTimeByHeapDijkstra composes this repo's own Heap<Element,TOrder>, ordered
// by ByPriorityOrder<TNode,TWeight> - the same Dijkstra-shaped frontier
// TrappingRainWaterIISolution.TrapRainWaterByHeapFloodFill uses, here keyed by
// the max elevation crossed so far instead of a flood-fill water level.
// MinTimeByBinarySearchFloodFill is the textbook alternative: binary search the
// answer, re-scanning the whole grid with a fresh BFS per candidate time.
internal static class SwimInRisingWaterSolution
{
    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // Deliberately written without this repo's primitives - a plain BCL
    // Queue<(int,int)> flood-fill per candidate time is what you would write
    // without this repo.
    public static int MinTimeByBinarySearchFloodFill(int[][] grid)
    {
        var n = grid.Length;
        var lo = 0;
        var hi = (n * n) - 1;

        while (lo < hi)
        {
            var mid = lo + ((hi - lo) / 2);

            if (CanReachAtTime(grid, mid, n))
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

    private static bool CanReachAtTime(int[][] grid, int time, int n)
    {
        if (grid[0][0] > time)
        {
            return false;
        }

        var walk = new FloodWalk(new bool[n, n], new Queue<(int Row, int Col)>());
        walk.Queue.Enqueue((0, 0));
        walk.Visited[0, 0] = true;

        return FloodReachesCorner(grid, time, walk);
    }

    // Spread the flood from the cells already queued, reporting whether the
    // bottom-right corner is reached before the queue empties.
    private static bool FloodReachesCorner(int[][] grid, int time, FloodWalk walk)
    {
        var n = grid.Length;

        while (walk.Queue.Count > 0)
        {
            var cell = walk.Queue.Dequeue();

            if (cell.Row == n - 1 && cell.Col == n - 1)
            {
                return true;
            }

            EnqueueReachableNeighbors(grid, cell, time, walk);
        }

        return false;
    }

    // The cell being expanded arrives as one argument rather than as a row and a column,
    // and the flood's own reached and pending sets as the one walk every step of the
    // flood both reads and writes; the grid's size is read off the grid rather than
    // passed in alongside it.
    private static void EnqueueReachableNeighbors(int[][] grid, (int Row, int Col) cell, int time, FloodWalk walk)
    {
        var n = grid.Length;

        foreach (var (dr, dc) in Directions)
        {
            var nr = cell.Row + dr;
            var nc = cell.Col + dc;

            if (IsOutsideGrid(nr, nc, n) || IsClosedToTheFlood((nr, nc), grid, time, walk.Visited))
            {
                continue;
            }

            walk.Visited[nr, nc] = true;
            walk.Queue.Enqueue((nr, nc));
        }
    }

    // A neighbour the flood has already reached, or whose cell the water has not yet
    // risen to at this time, cannot be entered now.
    private static bool IsClosedToTheFlood((int Row, int Col) cell, int[][] grid, int time, bool[,] visited)
        => visited[cell.Row, cell.Col] || grid[cell.Row][cell.Col] > time;

    // Every cell reached is pushed at the max elevation crossed so far; popping
    // the globally cheapest frontier cell first guarantees each cell is first
    // reached at its true minimum bottleneck cost - the same settle-once
    // argument Dijkstra's own first-pop-is-final proof relies on.
    public static int MinTimeByHeapDijkstra(int[][] grid)
    {
        var n = grid.Length;
        var visited = new bool[n, n];
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();

        frontier.Push(((0, 0), grid[0][0]));
        visited[0, 0] = true;

        return DrainFrontier(grid, visited, frontier);
    }

    // The settle-once drain itself: pop the cheapest entry, and either the
    // bottom-right corner's first pop ends the search with its final time, or the
    // entry relaxes its unvisited neighbours at the max elevation crossed so far.
    private static int DrainFrontier(
        int[][] grid,
        bool[,] visited,
        Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> frontier)
    {
        var n = grid.Length;

        while (frontier.TryPop(out var entry))
        {
            var (row, col) = entry.Node;

            if (row == n - 1 && col == n - 1)
            {
                var time = entry.Priority;
                return time;
            }

            ExploreNeighbors(grid, entry, visited, frontier);
        }

        // Every cell in an n x n grid is orthogonally connected to (0,0), so the
        // frontier always reaches the bottom-right corner before emptying.
        throw new InvalidOperationException("Unreachable for a valid n x n grid: every cell connects to (0,0).");
    }

    private static void ExploreNeighbors(
        int[][] grid,
        ((int Row, int Col) Node, int Priority) entry,
        bool[,] visited,
        Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> frontier)
    {
        var (row, col) = entry.Node;
        var time = entry.Priority;
        var n = grid.Length;

        foreach (var (dr, dc) in Directions)
        {
            var nr = row + dr;
            var nc = col + dc;

            if (IsOutsideGrid(nr, nc, n) || visited[nr, nc])
            {
                continue;
            }

            visited[nr, nc] = true;
            frontier.Push(((nr, nc), Math.Max(time, grid[nr][nc])));
        }
    }

    // Off the grid on any of its four edges - there is no cell to swim into.
    // Shared by both arms' neighbour scans, so it sits last.
    private static bool IsOutsideGrid(int row, int col, int size)
        => row < 0 || row >= size || col < 0 || col >= size;

    // The reached set and the pending set of one flood-fill scan at a candidate
    // time - the pair every step of the flood both reads and writes, so it travels
    // as one argument rather than as two.
    private readonly record struct FloodWalk(bool[,] Visited, Queue<(int Row, int Col)> Queue);
}
