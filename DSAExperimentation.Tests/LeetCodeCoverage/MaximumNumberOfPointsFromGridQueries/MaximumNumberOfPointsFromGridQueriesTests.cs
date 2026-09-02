using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfPointsFromGridQueries;

// LeetCode 2503. Maximum Number of Points From Grid Queries: for a given query,
// the optimal play only ever steps onto a cell whose value is below the query
// (stepping onto anything else ends the whole process with no benefit), so the
// answer is exactly the size of the 4-directionally-connected region of
// below-query cells reachable from (0,0). Sorting queries ascending and flood-
// filling outward with a min-heap ordered by cell value - Heap<Element,TOrder>
// closed over ByPriorityOrder<GridNode,int>, the same frontier shape
// ShortestPath.Dijkstra uses - visits cells in exactly the order they'd first
// become reachable at an increasing threshold, so each query's answer is just
// how many cells the heap has popped once its smallest remaining value stops
// being < that query. Neighbor iteration reuses Graph.Grids' GridNode/
// GridChildren/GridTopology as pure 4-directional bounds arithmetic over an
// all-passable Grid (the value threshold, not a grid obstacle, is what gates
// movement here), with a HashSet<GridNode> tracking visited cells the same way
// ShortestPath.SearchState.Settled does.
public sealed partial class MaximumNumberOfPointsFromGridQueriesTests
{
    // Hand-traced grids (see the walkthrough in this suite's PR/notes): each
    // expected value was worked out cell-by-cell rather than copied from memory.
    public static TheoryData<int[][], int[], int[]> Examples =>
        new()
        {
            // 1 3 / 2 4 - query 2 only ever admits the start cell (1); query 5
            // admits every cell, since the whole 2x2 grid is < 5.
            { [[1, 3], [2, 4]], [2, 5], [1, 4] },
            // Single cell: strictly-greater-than is required, so a query equal to
            // the only value gets 0 points, one greater gets the single point.
            { [[1]], [1, 2], [0, 1] },
            // 3 1 2 (one row): start value 3 blocks both query<=3 cases outright;
            // query 10 floods the whole row.
            { [[3, 1, 2]], [2, 3, 10], [0, 0, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPoints_HandTracedGrids_ReturnsPerQueryReachableCellCounts(
        int[][] grid, int[] queries, int[] expected)
    {
        Assert.Equal(expected, MaxPoints(grid, queries));
    }

    private static int[] MaxPoints(int[][] gridValues, int[] queries)
    {
        var rows = gridValues.Length;
        var cols = gridValues[0].Length;
        var grid = new Grid(AllPassable(rows, cols));

        var start = new GridNode(0, 0, grid);
        var visited = new HashSet<GridNode> { start };
        var frontier = new Heap<(GridNode Node, int Priority), ByPriorityOrder<GridNode, int>>();
        frontier.Push((start, gridValues[0][0]));

        var order = queries
            .Select((value, index) => (Value: value, Index: index))
            .OrderBy(query => query.Value);

        var answers = new int[queries.Length];
        var points = 0;

        foreach (var query in order)
        {
            points = AdvanceFrontier(frontier, visited, gridValues, query.Value, points);
            answers[query.Index] = points;
        }

        return answers;
    }

    private static int AdvanceFrontier(
        Heap<(GridNode Node, int Priority), ByPriorityOrder<GridNode, int>> frontier,
        HashSet<GridNode> visited, int[][] gridValues, int query, int points)
    {
        while (frontier.TryPeek(out var top) && top.Priority < query)
        {
            frontier.TryPop(out var current);
            points++;

            var children = GridTopology.GetChildren(current.Node);

            for (var i = 0; i < children.Count; i++)
            {
                var neighbor = children.Get(i);

                if (visited.Add(neighbor))
                {
                    frontier.Push((neighbor, gridValues[neighbor.Row][neighbor.Col]));
                }
            }
        }

        return points;
    }

    private static bool[,] AllPassable(int rows, int cols)
    {
        var passable = new bool[rows, cols];

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                passable[row, col] = true;
            }
        }

        return passable;
    }
}
