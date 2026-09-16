using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.FindASafeWalkThroughAGrid;

// LeetCode 3286. Find a Safe Walk Through a Grid: walking onto a cell with
// grid[i][j] == 1 costs one health point, including the starting cell (0, 0)
// itself. The best achievable health at (m - 1, n - 1) is "health minus the
// fewest 1-cells any path from (0,0) to (m-1,n-1) has to cross", which is a
// single-pair shortest-path query on a 4-directionally connected grid whose edge
// weight is the DESTINATION cell's own 0/1 cost - not WeightedGrid's fixed
// unit-weight edges (a different edge-weight rule, so this problem builds its
// own graph, the same way LockGraph builds its own Cayley graph rather than
// reusing WeightedGrid's).
//
// Both strategies solve that shortest-path query; they differ in how. The
// composed strategy hands the very same edge-weighted graph shape
// (WeightedGridNode/WeightedGridTopology) to this repo's own
// ShortestPath.Dijkstra. The baseline is the textbook O((mn)^2) array Dijkstra
// with no priority queue at all - deliberately a different complexity class, not
// just a different implementation, since it is the arm ShortestPath's Heap-backed
// version has to justify itself against.
internal static class FindASafeWalkThroughAGridSolution
{
    private static readonly (int DeltaRow, int DeltaCol)[] Orthogonal = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // Textbook baseline: Dijkstra with a plain O(V) linear scan for the next
    // unsettled minimum instead of a heap, over a flat int[] distance array - no
    // repo primitives at all.
    public static bool IsSafeByBruteForceArrayDijkstra(int[][] grid, int health)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var total = rows * cols;
        var damage = ArrayScanDijkstra(grid, rows, cols);

        var targetDamage = damage[total - 1];
        return targetDamage != int.MaxValue && health - targetDamage >= 1;
    }

    // The search itself: settle the nearest reachable cell `total` times over, so
    // that every cell's cheapest damage is final, and hand back the damage array.
    private static int[] ArrayScanDijkstra(int[][] grid, int rows, int cols)
    {
        var total = rows * cols;
        var damage = new int[total];

        Array.Fill(damage, int.MaxValue);
        damage[0] = grid[0][0];
        var settled = new bool[total];

        for (var iteration = 0; iteration < total; iteration++)
        {
            var current = NextUnsettledMinimum(damage, settled);

            if (current == -1)
            {
                break;
            }

            settled[current] = true;
            RelaxArrayNeighbors(current, (rows, cols), grid, damage);
        }

        return damage;
    }

    private static int NextUnsettledMinimum(int[] damage, bool[] settled)
    {
        var current = -1;

        for (var i = 0; i < damage.Length; i++)
        {
            if (IsBetterCandidate(i, damage, settled, current))
            {
                current = i;
            }
        }

        return current;
    }

    // The next vertex to settle is the nearest cell that is still unsettled, still
    // reachable, and closer than whatever the scan is holding.
    private static bool IsBetterCandidate(int index, int[] damage, bool[] settled, int current)
        => !settled[index] && damage[index] != int.MaxValue && (current == -1 || damage[index] < damage[current]);

    // Off the grid on any of its four edges - the step has no cell to land on.
    private static bool IsOutside(int row, int col, int rows, int cols)
        => row < 0 || row >= rows || col < 0 || col >= cols;

    // `current` is a flat index, so the grid's shape is what turns it back into a cell -
    // the two bounds travel as that one shape rather than as two loose ints beside the grid.
    private static void RelaxArrayNeighbors(int current, (int Rows, int Cols) size, int[][] grid, int[] damage)
    {
        var row = current / size.Cols;
        var col = current % size.Cols;

        foreach (var (deltaRow, deltaCol) in Orthogonal)
        {
            var nextRow = row + deltaRow;
            var nextCol = col + deltaCol;

            if (IsOutside(nextRow, nextCol, size.Rows, size.Cols))
            {
                continue;
            }

            var next = (nextRow * size.Cols) + nextCol;
            var candidate = damage[current] + grid[nextRow][nextCol];

            if (candidate < damage[next])
            {
                damage[next] = candidate;
            }
        }
    }

    // This repo's own composition: a cell-cost-weighted WeightedGridNode graph
    // (built below, since WeightedGrid.Build's edges are fixed at weight 1) run
    // through ShortestPath.Dijkstra.
    public static bool IsSafeByWeightedGridDijkstra(int[][] grid, int health)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var nodes = BuildCellCostGraph(grid, rows, cols);

        return IsSafeByWeightedGridDijkstra(nodes, (rows, cols), grid[0][0], health);
    }

    // The prepared graph and the grid size it was built for travel together: the size is
    // what addresses the destination cell in `nodes`, not an independent value.
    public static bool IsSafeByWeightedGridDijkstra(
        Dictionary<(int Row, int Col), WeightedGridNode> nodes, (int Rows, int Cols) size, int startCost, int health)
    {
        var source = nodes[(0, 0)];
        var target = nodes[(size.Rows - 1, size.Cols - 1)];

        var distances = ShortestPath.Dijkstra<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int>(source);

        if (!distances.TryGetValue(target, out var pathDamage))
        {
            return false;
        }

        return HasHealthToSpare(startCost, pathDamage, health);
    }

    // Dijkstra's own distances never include the source's cost, since every edge
    // weight it sums is charged to entering the DESTINATION - so the source cell's
    // own damage (already paid just by starting there) is added back in before the
    // health left over is checked.
    private static bool HasHealthToSpare(int startCost, int pathDamage, int health)
    {
        var totalDamage = startCost + pathDamage;
        return health - totalDamage >= 1;
    }

    // The prepared input the composed strategy's hoisted overload takes: a
    // 4-directionally wired WeightedGridNode graph whose edge weight into a cell
    // is that cell's own grid value, so a benchmark's [GlobalSetup] can charge
    // graph construction to setup rather than the measured Dijkstra call.
    public static Dictionary<(int Row, int Col), WeightedGridNode> BuildCellCostGraph(int[][] grid, int rows, int cols)
    {
        var nodes = CreateGridNodes(rows, cols);
        WireCellCosts(nodes, grid);

        return nodes;
    }

    // Every cell of the grid as a node, carrying no edges yet.
    private static Dictionary<(int Row, int Col), WeightedGridNode> CreateGridNodes(int rows, int cols)
    {
        var nodes = new Dictionary<(int Row, int Col), WeightedGridNode>();

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                nodes[(row, col)] = new WeightedGridNode(row, col);
            }
        }

        return nodes;
    }

    // One 4-directional edge per neighbour, weighted by the DESTINATION cell's own
    // cost.
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
}
