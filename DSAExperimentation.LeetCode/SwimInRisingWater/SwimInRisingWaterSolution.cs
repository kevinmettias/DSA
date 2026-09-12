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

        var visited = new bool[n, n];
        var queue = new Queue<(int Row, int Col)>();
        queue.Enqueue((0, 0));
        visited[0, 0] = true;

        while (queue.Count > 0)
        {
            var (row, col) = queue.Dequeue();

            if (row == n - 1 && col == n - 1)
            {
                return true;
            }

            EnqueueReachableNeighbors(grid, row, col, time, n, visited, queue);
        }

        return false;
    }

    private static void EnqueueReachableNeighbors(
        int[][] grid, int row, int col, int time, int n, bool[,] visited, Queue<(int Row, int Col)> queue)
    {
        foreach (var (dr, dc) in Directions)
        {
            var nr = row + dr;
            var nc = col + dc;

            if (nr < 0 || nr >= n || nc < 0 || nc >= n || visited[nr, nc] || grid[nr][nc] > time)
            {
                continue;
            }

            visited[nr, nc] = true;
            queue.Enqueue((nr, nc));
        }
    }

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

        while (frontier.TryPop(out var entry))
        {
            var (row, col) = entry.Node;
            var time = entry.Priority;

            if (row == n - 1 && col == n - 1)
            {
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

            if (nr < 0 || nr >= n || nc < 0 || nc >= n || visited[nr, nc])
            {
                continue;
            }

            visited[nr, nc] = true;
            frontier.Push(((nr, nc), Math.Max(time, grid[nr][nc])));
        }
    }
}
