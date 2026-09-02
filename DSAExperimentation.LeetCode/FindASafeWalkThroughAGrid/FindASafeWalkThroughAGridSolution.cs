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
            RelaxArrayNeighbors(current, rows, cols, grid, damage);
        }

        var targetDamage = damage[total - 1];
        return targetDamage != int.MaxValue && health - targetDamage >= 1;
    }

    private static int NextUnsettledMinimum(int[] damage, bool[] settled)
    {
        var current = -1;

        for (var i = 0; i < damage.Length; i++)
        {
            if (!settled[i] && damage[i] != int.MaxValue && (current == -1 || damage[i] < damage[current]))
            {
                current = i;
            }
        }

        return current;
    }

    private static void RelaxArrayNeighbors(int current, int rows, int cols, int[][] grid, int[] damage)
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

        return IsSafeByWeightedGridDijkstra(nodes, rows, cols, grid[0][0], health);
    }

    public static bool IsSafeByWeightedGridDijkstra(
        Dictionary<(int Row, int Col), WeightedGridNode> nodes, int rows, int cols, int startCost, int health)
    {
        var source = nodes[(0, 0)];
        var target = nodes[(rows - 1, cols - 1)];

        var distances = ShortestPath.Dijkstra<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int>(source);

        if (!distances.TryGetValue(target, out var pathDamage))
        {
            return false;
        }

        // Dijkstra's own distances never include the source's cost, since every
        // edge weight it sums is charged to entering the DESTINATION - so the
        // source cell's own damage (already paid just by starting there) is
        // added back in here.
        var totalDamage = startCost + pathDamage;
        return health - totalDamage >= 1;
    }

    // The prepared input the composed strategy's hoisted overload takes: a
    // 4-directionally wired WeightedGridNode graph whose edge weight into a cell
    // is that cell's own grid value, so a benchmark's [GlobalSetup] can charge
    // graph construction to setup rather than the measured Dijkstra call.
    public static Dictionary<(int Row, int Col), WeightedGridNode> BuildCellCostGraph(int[][] grid, int rows, int cols)
    {
        var nodes = new Dictionary<(int Row, int Col), WeightedGridNode>();

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
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
