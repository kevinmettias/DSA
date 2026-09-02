using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

using RepoCellQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>;

namespace DSAExperimentation.LeetCode.FindTheSafestPathInAGrid;

// LeetCode 2812. Find the Safest Path in a Grid: a cell's safeness factor is its
// Manhattan distance to the nearest thief (a 1 cell). Find a path from (0,0) to
// (n-1,n-1) that maximizes the MINIMUM safeness factor visited along the way -
// the classic "maximum bottleneck path" problem.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm. Repo Queue<T> is aliased
// (RepoCellQueue) rather than used via a bare `using` + `new Queue<T>()`: this
// project's global `using System.Collections.Generic;` makes a bare `Queue<T>`
// ambiguous (CS0104) against the BCL's own Queue<T> - see FinalStringByDeque's
// sibling problems for the same collision on Stack<T>.
internal static class FindTheSafestPathInAGridSolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // Baseline: safeness via a hand-rolled multi-source BFS over a BCL
    // Queue<(int,int)>, then binary search over the safeness threshold with a
    // hand-rolled single-source BFS reachability check each step - "what you'd
    // write without this repo".
    public static int MaximumSafenessFactorByBclBfs(int[][] grid)
    {
        var safeness = ComputeSafenessByBclBfs(grid);
        var n = grid.Length;

        return BinarySearchMaxThreshold(safeness, n, threshold => IsReachableAtThresholdByBclBfs(safeness, n, threshold));
    }

    // Composed: a single-pass modified Dijkstra (no binary search) over this
    // repo's Heap<Element,TOrder> - the same frontier ShortestPath.Dijkstra and
    // MaximumNumberOfPointsFromGridQueriesTests already use, just maximizing
    // instead of minimizing (achieved by negating the priority, since
    // ByPriorityOrder<TNode,TWeight> is a fixed min-heap comparison) - "pop the
    // node with the best bottleneck reached so far" is exactly Dijkstra's
    // "pop the node with the smallest distance reached so far", just flipped.
    // GridTopology/GridChildren/GridNode/Grid supply 4-directional neighbor
    // iteration over an all-passable grid (the safeness threshold isn't a grid
    // obstacle here, so nothing needs an obstacle-aware Grid - the same move
    // MaximumNumberOfPointsFromGridQueriesTests makes). Safeness itself still
    // needs its own multi-source BFS, run over this repo's own Queue<T>.
    public static int MaximumSafenessFactorByHeap(int[][] grid)
    {
        var safeness = ComputeSafenessByRepoQueue(grid);
        var n = grid.Length;
        var passableGrid = new Grid(AllPassable(n));

        var start = new GridNode(0, 0, passableGrid);
        var bestBottleneck = new Dictionary<GridNode, int> { [start] = safeness[0][0] };
        var settled = new HashSet<GridNode>();
        var frontier = new Heap<(GridNode Node, int Priority), ByPriorityOrder<GridNode, int>>();
        frontier.Push((start, -safeness[0][0]));

        while (frontier.TryPop(out var entry))
        {
            var bottleneck = -entry.Priority;

            if (!settled.Add(entry.Node))
            {
                continue;
            }

            if (entry.Node.Row == n - 1 && entry.Node.Col == n - 1)
            {
                return bottleneck;
            }

            RelaxNeighbors(entry.Node, bottleneck, safeness, bestBottleneck, frontier);
        }

        // Unreachable in practice: the grid is fully connected (AllPassable), so
        // (n-1,n-1) is always eventually popped.
        return 0;
    }

    private static void RelaxNeighbors(
        GridNode node,
        int bottleneck,
        int[][] safeness,
        Dictionary<GridNode, int> bestBottleneck,
        Heap<(GridNode Node, int Priority), ByPriorityOrder<GridNode, int>> frontier)
    {
        var children = GridTopology.GetChildren(node);

        for (var i = 0; i < children.Count; i++)
        {
            var neighbor = children.Get(i);
            var candidate = Math.Min(bottleneck, safeness[neighbor.Row][neighbor.Col]);

            if (bestBottleneck.TryGetValue(neighbor, out var known) && candidate <= known)
            {
                continue;
            }

            bestBottleneck[neighbor] = candidate;
            frontier.Push((neighbor, -candidate));
        }
    }

    private static int BinarySearchMaxThreshold(int[][] safeness, int n, Func<int, bool> isFeasible)
    {
        var low = 0;
        var high = 0;

        for (var r = 0; r < n; r++)
        {
            for (var c = 0; c < n; c++)
            {
                high = Math.Max(high, safeness[r][c]);
            }
        }

        var answer = 0;

        while (low <= high)
        {
            var mid = low + ((high - low) / 2);

            if (isFeasible(mid))
            {
                answer = mid;
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        return answer;
    }

    private static bool IsReachableAtThresholdByBclBfs(int[][] safeness, int n, int threshold)
    {
        if (safeness[0][0] < threshold || safeness[n - 1][n - 1] < threshold)
        {
            return false;
        }

        var visited = new bool[n, n];
        visited[0, 0] = true;
        var queue = new System.Collections.Generic.Queue<(int Row, int Col)>();
        queue.Enqueue((0, 0));

        while (queue.Count > 0)
        {
            var (row, col) = queue.Dequeue();

            if (row == n - 1 && col == n - 1)
            {
                return true;
            }

            foreach (var (dRow, dCol) in Directions)
            {
                var (nextRow, nextCol) = (row + dRow, col + dCol);

                if (nextRow < 0 || nextRow >= n || nextCol < 0 || nextCol >= n
                    || visited[nextRow, nextCol] || safeness[nextRow][nextCol] < threshold)
                {
                    continue;
                }

                visited[nextRow, nextCol] = true;
                queue.Enqueue((nextRow, nextCol));
            }
        }

        return false;
    }

    private static int[][] ComputeSafenessByBclBfs(int[][] grid)
    {
        var n = grid.Length;
        var safeness = new int[n][];
        var queue = new System.Collections.Generic.Queue<(int Row, int Col)>();

        for (var r = 0; r < n; r++)
        {
            safeness[r] = new int[n];

            for (var c = 0; c < n; c++)
            {
                safeness[r][c] = grid[r][c] == 1 ? 0 : -1;

                if (grid[r][c] == 1)
                {
                    queue.Enqueue((r, c));
                }
            }
        }

        while (queue.Count > 0)
        {
            var (row, col) = queue.Dequeue();

            foreach (var (dRow, dCol) in Directions)
            {
                var (nextRow, nextCol) = (row + dRow, col + dCol);

                if (nextRow < 0 || nextRow >= n || nextCol < 0 || nextCol >= n || safeness[nextRow][nextCol] != -1)
                {
                    continue;
                }

                safeness[nextRow][nextCol] = safeness[row][col] + 1;
                queue.Enqueue((nextRow, nextCol));
            }
        }

        return safeness;
    }

    private static int[][] ComputeSafenessByRepoQueue(int[][] grid)
    {
        var n = grid.Length;
        var safeness = new int[n][];
        var frontier = new RepoCellQueue();

        for (var r = 0; r < n; r++)
        {
            safeness[r] = new int[n];

            for (var c = 0; c < n; c++)
            {
                safeness[r][c] = grid[r][c] == 1 ? 0 : -1;

                if (grid[r][c] == 1)
                {
                    frontier.Enqueue((r, c));
                }
            }
        }

        while (frontier.TryDequeue(out var cell))
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var (nextRow, nextCol) = (cell.Row + dRow, cell.Col + dCol);

                if (nextRow < 0 || nextRow >= n || nextCol < 0 || nextCol >= n || safeness[nextRow][nextCol] != -1)
                {
                    continue;
                }

                safeness[nextRow][nextCol] = safeness[cell.Row][cell.Col] + 1;
                frontier.Enqueue((nextRow, nextCol));
            }
        }

        return safeness;
    }

    private static bool[,] AllPassable(int n)
    {
        var passable = new bool[n, n];

        for (var r = 0; r < n; r++)
        {
            for (var c = 0; c < n; c++)
            {
                passable[r, c] = true;
            }
        }

        return passable;
    }
}
