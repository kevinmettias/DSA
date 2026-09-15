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
        var removals = ArrayScanRemovals(grid, rows, cols);

        return removals[(rows * cols) - 1];
    }

    // The search itself: settle the nearest reachable cell once per cell, so every
    // cell's cheapest removal count is final, and hand back the removals array.
    private static int[] ArrayScanRemovals(int[][] grid, int rows, int cols)
    {
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
            RelaxArrayNeighbors(current, grid, removals);
        }

        return removals;
    }

    private static int NextUnsettledMinimum(int[] removals, bool[] settled)
    {
        var current = -1;

        for (var i = 0; i < removals.Length; i++)
        {
            if (!settled[i] && IsImprovement(removals, i, current))
            {
                current = i;
            }
        }

        return current;
    }

    // An unsettled cell improves the scan once it has been reached at all, and then
    // only by carrying fewer removals than the best found so far - a cell nothing
    // has picked yet counting as beatable.
    private static bool IsImprovement(int[] removals, int i, int current)
        => removals[i] != int.MaxValue
            && (current == -1 || removals[i] < removals[current]);

    private static void RelaxArrayNeighbors(int current, int[][] grid, int[] removals)
    {
        var shape = (Rows: grid.Length, Cols: grid[0].Length);

        foreach (var (deltaRow, deltaCol) in Orthogonal)
        {
            RelaxNeighbour(current, (grid, removals), shape, (deltaRow, deltaCol));
        }
    }

    // One 4-directionally adjacent cell of `current`: a step that leaves the grid is
    // dropped, an in-grid one relaxes that neighbour's removal count.
    private static void RelaxNeighbour(
        int current,
        (int[][] Grid, int[] Removals) state,
        (int Rows, int Cols) shape,
        (int DeltaRow, int DeltaCol) step)
    {
        var row = (current / shape.Cols) + step.DeltaRow;
        var col = (current % shape.Cols) + step.DeltaCol;

        if (!IsInside(row, col, shape.Rows, shape.Cols))
        {
            return;
        }

        var next = (row * shape.Cols) + col;
        var candidate = state.Removals[current] + state.Grid[row][col];

        if (candidate < state.Removals[next])
        {
            state.Removals[next] = candidate;
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
        var nodes = CreateGridNodes(grid);
        WireCellCosts(nodes, grid);

        return nodes;
    }

    // Every cell of `grid` as a node, carrying no edges yet.
    private static Dictionary<(int Row, int Col), WeightedGridNode> CreateGridNodes(int[][] grid)
    {
        var nodes = new Dictionary<(int Row, int Col), WeightedGridNode>();

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                nodes[(row, col)] = new WeightedGridNode(row, col);
            }
        }

        return nodes;
    }

    // One 4-directional edge per neighbour, weighted by the DESTINATION cell's own
    // obstacle flag.
    private static void WireCellCosts(Dictionary<(int Row, int Col), WeightedGridNode> nodes, int[][] grid)
    {
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
    }

    // Both coordinates within the grid is one idea, and the relaxation step asks for
    // its negation.
    private static bool IsInside(int row, int col, int rows, int cols)
        => row >= 0 && row < rows && col >= 0 && col < cols;
}
