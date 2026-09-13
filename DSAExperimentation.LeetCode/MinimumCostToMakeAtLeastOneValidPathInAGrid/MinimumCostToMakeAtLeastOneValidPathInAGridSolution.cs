using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.MinimumCostToMakeAtLeastOneValidPathInAGrid;

// LeetCode 1368. Minimum Cost to Make at Least One Valid Path in a Grid: each
// cell's arrow (1=right, 2=left, 3=down, 4=up) is free to follow and costs 1 to
// override - exactly a 0/1-weighted shortest path from (0,0) to the far corner,
// with settle-once bookkeeping (a distances map plus a settled set, mirroring
// ShortestPath.cs's own Explore loop) since a plain "mark visited when pushed"
// shortcut is only sound for minimax costs, not additive ones.
//
// The two strategies differ only in how the frontier picks the next cell to
// settle: a full linear scan of the unsettled distance table (textbook O(V^2)
// Dijkstra) or this repo's own Heap<Element,TOrder> ordered by
// ByPriorityOrder<TNode,TWeight> - the same heap-backed frontier
// SwimInRisingWater uses - giving O(E log V).
internal static class MinimumCostToMakeAtLeastOneValidPathInAGridSolution
{
    // Indexed so that Directions[d] is the move the arrow value d + 1 asks for:
    // 1=right, 2=left, 3=down, 4=up.
    private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];

    // The textbook answer: no priority queue at all, just a distance table that is
    // linear-scanned for the current unsettled minimum every round. Deliberately
    // written with nothing but BCL arrays - it is the arm the heap-backed frontier
    // below has to justify itself against.
    public static int MinCostByLinearScanDijkstra(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var distances = new int?[rows, cols];
        var settled = new bool[rows, cols];
        distances[0, 0] = 0;

        for (var round = 0; round < rows * cols; round++)
        {
            var cell = FindUnsettledMinimum(distances, settled);

            if (cell.Row < 0)
            {
                break;
            }

            if (SettleAndRelax(grid, cell, distances, settled))
            {
                return distances[cell.Row, cell.Col]!.Value;
            }
        }

        return distances[rows - 1, cols - 1]!.Value;
    }

    private static bool SettleAndRelax(int[][] grid, (int Row, int Col) cell, int?[,] distances, bool[,] settled)
    {
        settled[cell.Row, cell.Col] = true;

        if (cell.Row == grid.Length - 1 && cell.Col == grid[0].Length - 1)
        {
            return true;
        }

        RelaxNeighborsIntoTable(grid, cell, distances, settled);
        return false;
    }

    private static void RelaxNeighborsIntoTable(int[][] grid, (int Row, int Col) cell, int?[,] distances, bool[,] settled)
    {
        for (var direction = 0; direction < Directions.Length; direction++)
        {
            var next = Step(cell, direction);

            if (!IsInside(grid, next) || settled[next.Row, next.Col])
            {
                continue;
            }

            var candidate = distances[cell.Row, cell.Col]!.Value + CrossingCost(grid, cell, direction);

            if (distances[next.Row, next.Col] is not int known || candidate < known)
            {
                distances[next.Row, next.Col] = candidate;
            }
        }
    }

    private static (int Row, int Col) FindUnsettledMinimum(int?[,] distances, bool[,] settled)
    {
        var best = int.MaxValue;
        var result = (-1, -1);

        for (var row = 0; row < distances.GetLength(0); row++)
        {
            for (var col = 0; col < distances.GetLength(1); col++)
            {
                if (!settled[row, col] && distances[row, col] is int distance && distance < best)
                {
                    best = distance;
                    result = (row, col);
                }
            }
        }

        return result;
    }

    // This repo's own frontier: Heap<Element,TOrder> ordered by ByPriorityOrder, so
    // the next cell to settle is a pop rather than a scan of the whole table.
    public static int MinCostByHeapDijkstra(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
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

            RelaxNeighborsIntoFrontier(grid, entry.Node, distances, frontier);
        }

        return distances[(rows - 1, cols - 1)];
    }

    private static void RelaxNeighborsIntoFrontier(
        int[][] grid,
        (int Row, int Col) cell,
        Dictionary<(int Row, int Col), int> distances,
        Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> frontier)
    {
        for (var direction = 0; direction < Directions.Length; direction++)
        {
            var next = Step(cell, direction);

            if (!IsInside(grid, next))
            {
                continue;
            }

            var candidate = distances[cell] + CrossingCost(grid, cell, direction);

            if (!distances.TryGetValue(next, out var known) || candidate < known)
            {
                distances[next] = candidate;
                frontier.Push((next, candidate));
            }
        }
    }

    private static (int Row, int Col) Step((int Row, int Col) cell, int direction)
    {
        var (dRow, dCol) = Directions[direction];

        return (cell.Row + dRow, cell.Col + dCol);
    }

    // Following the source cell's own arrow is free; any other direction costs one
    // override.
    private static int CrossingCost(int[][] grid, (int Row, int Col) cell, int direction)
        => grid[cell.Row][cell.Col] == direction + 1 ? 0 : 1;

    private static bool IsInside(int[][] grid, (int Row, int Col) cell)
        => cell.Row >= 0 && cell.Row < grid.Length && cell.Col >= 0 && cell.Col < grid[0].Length;
}
