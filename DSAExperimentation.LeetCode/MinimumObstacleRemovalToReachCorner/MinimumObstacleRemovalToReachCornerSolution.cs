using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.MinimumObstacleRemovalToReachCorner;

// LeetCode 2290. Minimum Obstacle Removal to Reach Corner: stepping onto a cell
// holding a 1 costs one removal, stepping onto a 0 costs nothing, so the fewest
// obstacles that have to be removed to walk from (0, 0) to (m - 1, n - 1) is a
// single-pair shortest-path query on a 4-directionally connected grid whose edge
// weight is the DESTINATION cell's own 0/1 value - not WeightedGrid's fixed
// unit-weight edges, so this problem wires its own graph (the same reason LC 3286
// does, and the same reason LockGraph builds its own Cayley graph).
//
// The start cell is guaranteed to be a 0, and Dijkstra charges every weight to
// entering a cell, so the distance it reports at the far corner IS the answer
// with nothing added back.
//
// Both strategies answer that query; they differ in how. The composed strategy
// hands the edge-weighted grid (WeightedGridNode/WeightedGridTopology) to this
// repo's own Heap-backed ShortestPath.Dijkstra. The baseline is the textbook
// O((mn)^2) Dijkstra with no priority queue at all, scanning a flat int[] for the
// next unsettled minimum - a different complexity class, which is the point of
// having it.
internal static class MinimumObstacleRemovalToReachCornerSolution
{
    private static readonly (int DeltaRow, int DeltaCol)[] Orthogonal = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // Textbook baseline: Dijkstra whose frontier is a linear scan over a flat
    // distance array instead of a heap. BCL only, deliberately - it is the arm
    // ShortestPath.Dijkstra has to justify itself against.
    public static int MinimumObstaclesByArrayScanDijkstra(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var total = rows * cols;
        var removals = new int[total];

        Array.Fill(removals, int.MaxValue);
        removals[0] = 0;
        var settled = new bool[total];

        for (var iteration = 0; iteration < total; iteration++)
        {
            var current = NextUnsettledMinimum(removals, settled);

            if (current == -1)
            {
                break;
            }

            settled[current] = true;
            RelaxArrayNeighbors(current, rows, cols, grid, removals);
        }

        return removals[total - 1];
    }

    private static int NextUnsettledMinimum(int[] removals, bool[] settled)
    {
        var current = -1;

        for (var i = 0; i < removals.Length; i++)
        {
            if (!settled[i] && removals[i] != int.MaxValue && (current == -1 || removals[i] < removals[current]))
            {
                current = i;
            }
        }

        return current;
    }

    private static void RelaxArrayNeighbors(int current, int rows, int cols, int[][] grid, int[] removals)
    {
        var row = current / cols;
        var col = current % cols;

        foreach (var (deltaRow, deltaCol) in Orthogonal)
        {
            var nextRow = row + deltaRow;
            var nextCol = col + deltaCol;

            if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
            {
                continue;
            }

            var next = (nextRow * cols) + nextCol;
            var candidate = removals[current] + grid[nextRow][nextCol];

            if (candidate < removals[next])
            {
                removals[next] = candidate;
            }
        }
    }

    // This repo's own composition: an obstacle-cost-weighted WeightedGridNode
    // graph run through ShortestPath.Dijkstra, whose frontier is Collections.Heap.
    public static int MinimumObstaclesByWeightedGridDijkstra(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        return MinimumObstaclesByWeightedGridDijkstra(BuildObstacleCostGraph(grid), rows, cols);
    }

    public static int MinimumObstaclesByWeightedGridDijkstra(
        Dictionary<(int Row, int Col), WeightedGridNode> nodes, int rows, int cols)
    {
        var distances = ShortestPath.Dijkstra<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int>(nodes[(0, 0)]);

        return distances[nodes[(rows - 1, cols - 1)]];
    }

    // The prepared input the composed strategy's hoisted overload takes: a
    // 4-directionally wired WeightedGridNode graph whose edge weight into a cell
    // is that cell's own obstacle flag, so a benchmark's [GlobalSetup] can charge
    // graph construction to setup rather than to the measured Dijkstra call.
    public static Dictionary<(int Row, int Col), WeightedGridNode> BuildObstacleCostGraph(int[][] grid)
    {
        var nodes = new Dictionary<(int Row, int Col), WeightedGridNode>();

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                nodes[(row, col)] = new WeightedGridNode(row, col);
            }
        }

        foreach (var ((row, col), node) in nodes)
        {
            foreach (var (deltaRow, deltaCol) in Orthogonal)
            {
                var neighborRow = row + deltaRow;
                var neighborCol = col + deltaCol;

                if (nodes.TryGetValue((neighborRow, neighborCol), out var neighbor))
                {
                    node.Edges.Add((grid[neighborRow][neighborCol], neighbor));
                }
            }
        }

        return nodes;
    }
}
