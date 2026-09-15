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

        return BinarySearchMaxThreshold(safeness, n);
    }

    private static int[][] ComputeSafenessByBclBfs(int[][] grid)
    {
        var n = grid.Length;
        var safeness = new int[n][];
        var queue = new System.Collections.Generic.Queue<(int Row, int Col)>(SeedSafeness(grid, safeness));

        SpreadSafenessByBclBfs(safeness, n, queue);

        return safeness;
    }

    // Multi-source BFS over the seed frontier: each pop settles a cell and gives
    // every still-unreached neighbour a distance one greater than its own.
    private static void SpreadSafenessByBclBfs(
        int[][] safeness,
        int n,
        System.Collections.Generic.Queue<(int Row, int Col)> queue)
    {
        while (queue.Count > 0)
        {
            var (row, col) = queue.Dequeue();

            foreach (var (dRow, dCol) in Directions)
            {
                var (nextRow, nextCol) = (row + dRow, col + dCol);

                if (IsOutsideGrid(nextRow, nextCol, n) || safeness[nextRow][nextCol] != -1)
                {
                    continue;
                }

                safeness[nextRow][nextCol] = safeness[row][col] + 1;
                queue.Enqueue((nextRow, nextCol));
            }
        }
    }

    private static int BinarySearchMaxThreshold(int[][] safeness, int n)
    {
        var high = MaxSafeness(safeness, n);
        var low = 0;
        var answer = 0;

        while (low <= high)
        {
            (low, high, answer) = NarrowThresholdRange(safeness, n, (low, high, answer));
        }

        return answer;
    }

    // No path can visit a cell safer than the safest cell, so the grid's maximum
    // safeness factor is a sound upper bound for the threshold search.
    private static int MaxSafeness(int[][] safeness, int gridSize)
    {
        var high = 0;

        for (var r = 0; r < gridSize; r++)
        {
            for (var c = 0; c < gridSize; c++)
            {
                high = Math.Max(high, safeness[r][c]);
            }
        }

        return high;
    }

    // One probe of the binary search for the largest threshold the walk still accepts:
    // a reachable midpoint raises the floor and becomes the best answer so far, an
    // unreachable one lowers the ceiling. Feasibility is monotone, since a lower
    // threshold can only open more cells.
    private static (int Low, int High, int Answer) NarrowThresholdRange(
        int[][] safeness, int gridSize, (int Low, int High, int Answer) range)
    {
        var (low, high, answer) = range;
        var mid = low + ((high - low) / 2);

        if (IsReachableAtThresholdByBclBfs(safeness, gridSize, mid))
        {
            answer = mid;
            low = mid + 1;
        }
        else
        {
            high = mid - 1;
        }

        return (low, high, answer);
    }

    private static bool IsReachableAtThresholdByBclBfs(int[][] safeness, int n, int threshold)
    {
        if (safeness[0][0] < threshold || safeness[n - 1][n - 1] < threshold)
        {
            return false;
        }

        var reached = new bool[n, n];
        reached[0, 0] = true;
        SpreadReachableCellsByBclBfs(safeness, n, threshold, reached);

        return reached[n - 1, n - 1];
    }

    // Flood-fill from (0,0) through every cell whose safeness still meets the
    // threshold; the caller reads the corner it cares about off the result.
    private static void SpreadReachableCellsByBclBfs(
        int[][] safeness,
        int n,
        int threshold,
        bool[,] reached)
    {
        var queue = new System.Collections.Generic.Queue<(int Row, int Col)>();
        queue.Enqueue((0, 0));

        while (queue.Count > 0)
        {
            var (row, col) = queue.Dequeue();

            foreach (var (dRow, dCol) in Directions)
            {
                var (nextRow, nextCol) = (row + dRow, col + dCol);

                if (IsEnteredOrOutsideGrid(nextRow, nextCol, n, reached)
                    || safeness[nextRow][nextCol] < threshold)
                {
                    continue;
                }

                reached[nextRow, nextCol] = true;
                queue.Enqueue((nextRow, nextCol));
            }
        }
    }

    // A neighbour this walk cannot enter: either it is off the board, or the walk has
    // already been there.
    private static bool IsEnteredOrOutsideGrid(int row, int col, int size, bool[,] visited) =>
        IsOutsideGrid(row, col, size) || visited[row, col];

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

        // Unreachable in practice: the grid is fully connected (AllPassable), so
        // (n-1,n-1) is always eventually popped.
        return DrainFrontierByBottleneck(frontier, settled, safeness, bestBottleneck);
    }

    private static int[][] ComputeSafenessByRepoQueue(int[][] grid)
    {
        var n = grid.Length;
        var safeness = new int[n][];
        var frontier = new RepoCellQueue();

        // This repo's Queue<T> has no constructor from a sequence (only Count,
        // Enqueue, TryPeek and TryDequeue), so the shared seed cells are enqueued
        // one at a time - same row-major order the BCL arm's constructor preserves.
        foreach (var (row, col) in SeedSafeness(grid, safeness))
        {
            frontier.Enqueue((row, col));
        }

        SpreadSafenessByRepoQueue(safeness, n, frontier);

        return safeness;
    }

    // The same multi-source BFS as SpreadSafenessByBclBfs, over this repo's own
    // Queue<T> instead of the BCL one.
    private static void SpreadSafenessByRepoQueue(int[][] safeness, int n, RepoCellQueue frontier)
    {
        while (frontier.TryDequeue(out var cell))
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var (nextRow, nextCol) = (cell.Row + dRow, cell.Col + dCol);

                if (IsOutsideGrid(nextRow, nextCol, n) || safeness[nextRow][nextCol] != -1)
                {
                    continue;
                }

                safeness[nextRow][nextCol] = safeness[cell.Row][cell.Col] + 1;
                frontier.Enqueue((nextRow, nextCol));
            }
        }
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

    // Pop the best-bottleneck node first, skipping any already settled, until the
    // bottom-right corner comes off the heap - that bottleneck is the answer.
    private static int DrainFrontierByBottleneck(
        Heap<(GridNode Node, int Priority), ByPriorityOrder<GridNode, int>> frontier,
        HashSet<GridNode> settled,
        int[][] safeness,
        Dictionary<GridNode, int> bestBottleneck)
    {
        var goal = safeness.Length - 1;

        while (frontier.TryPop(out var entry))
        {
            var bottleneck = -entry.Priority;

            if (!settled.Add(entry.Node))
            {
                continue;
            }

            if (entry.Node.Row == goal && entry.Node.Col == goal)
            {
                return bottleneck;
            }

            RelaxNeighbors((entry.Node, bottleneck), safeness, bestBottleneck, frontier);
        }

        return 0;
    }

    // The cell just settled and the bottleneck it was settled at are the one frontier
    // entry that was popped from the heap, so they arrive as one argument rather than
    // as two adjacent values a caller could transpose.
    private static void RelaxNeighbors(
        (GridNode Node, int Bottleneck) settled,
        int[][] safeness,
        Dictionary<GridNode, int> bestBottleneck,
        Heap<(GridNode Node, int Priority), ByPriorityOrder<GridNode, int>> frontier)
    {
        var children = GridTopology.GetChildren(settled.Node);

        for (var i = 0; i < children.Count; i++)
        {
            var neighbor = children.Get(i);
            var candidate = Math.Min(settled.Bottleneck, safeness[neighbor.Row][neighbor.Col]);

            if (bestBottleneck.TryGetValue(neighbor, out var known) && candidate <= known)
            {
                continue;
            }

            bestBottleneck[neighbor] = candidate;
            frontier.Push((neighbor, -candidate));
        }
    }

    // Both safeness grids start identically: every cell is 0 on a thief and -1
    // ("not reached yet") everywhere else, and the thieves come back as the seed
    // frontier for the multi-source BFS that follows.
    private static List<(int Row, int Col)> SeedSafeness(int[][] grid, int[][] safeness)
    {
        var n = grid.Length;
        var thieves = new List<(int Row, int Col)>();

        for (var r = 0; r < n; r++)
        {
            safeness[r] = new int[n];

            for (var c = 0; c < n; c++)
            {
                var isThief = grid[r][c] == 1;

                safeness[r][c] = isThief ? 0 : -1;

                if (grid[r][c] == 1)
                {
                    thieves.Add((r, c));
                }
            }
        }

        return thieves;
    }

    // Off the board is not a cell at all: both coordinates have to land inside the n-by-n grid.
    private static bool IsOutsideGrid(int row, int col, int size) =>
        row < 0 || row >= size || col < 0 || col >= size;
}
