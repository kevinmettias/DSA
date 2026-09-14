using DSAExperimentation.DataStructures.Graph.Grids;

using CellFrontier = DSAExperimentation.DataStructures.Heap.Heap<
    (DSAExperimentation.DataStructures.Graph.Grids.GridNode Node, int Priority),
    DSAExperimentation.DataStructures.Graph.ShortestPaths.ByPriorityOrder<
        DSAExperimentation.DataStructures.Graph.Grids.GridNode, int>>;

namespace DSAExperimentation.LeetCode.MaximumNumberOfPointsFromGridQueries;

// LeetCode 2503. Maximum Number of Points From Grid Queries: for a given query, the
// optimal play only ever steps onto a cell whose value is strictly below the query
// (stepping onto anything else ends the whole process with no benefit), so the
// answer is exactly the size of the 4-directionally-connected region of below-query
// cells reachable from (0,0).
//
// Both strategies answer the same question with the same signature - one int per
// query, in the queries' original order - so the test harness asserts them against
// the same examples and the benchmark harness times them against each other without
// either restating the algorithm.
internal static class MaximumNumberOfPointsFromGridQueriesSolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // Baseline: take the problem at its word and re-simulate every query
    // independently - a fresh 4-directional flood fill from (0,0) bounded by that
    // query's threshold, O(QueryCount * Rows * Cols) in total. A hand-rolled BCL
    // Stack and a bool[,] of visited cells, deliberately without this repo's
    // primitives (§17.5); it is the arm the shared-frontier strategy below has to
    // justify itself against.
    public static int[] MaxPointsByFloodFillPerQuery(int[][] grid, int[] queries)
    {
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = FloodFillCount(grid, queries[i]);
        }

        return answers;
    }

    private static int FloodFillCount(int[][] grid, int query)
    {
        var walk = new FloodWalk(
            grid, new bool[grid.Length, grid[0].Length], new Stack<(int Row, int Col)>(), query);

        // Nothing is reachable at all when the start cell is not itself below the
        // query, so the walk simply starts with an empty stack and scores 0.
        if (grid[0][0] < query)
        {
            walk.Visited[0, 0] = true;
            walk.Stack.Push((0, 0));
        }

        return DrainStack(walk);
    }

    private static int DrainStack(FloodWalk walk)
    {
        var points = 0;

        while (walk.Stack.Count > 0)
        {
            var cell = walk.Stack.Pop();
            points++;
            PushBelowQueryNeighbors(cell, walk);
        }

        return points;
    }

    private static void PushBelowQueryNeighbors((int Row, int Col) cell, FloodWalk walk)
    {
        var rows = walk.Grid.Length;
        var cols = walk.Grid[0].Length;

        foreach (var (dRow, dCol) in Directions)
        {
            var (nextRow, nextCol) = (cell.Row + dRow, cell.Col + dCol);

            var insideGrid = nextRow >= 0 && nextRow < rows && nextCol >= 0 && nextCol < cols;

            if (!insideGrid)
            {
                continue;
            }

            if (!walk.Visited[nextRow, nextCol] && walk.Grid[nextRow][nextCol] < walk.Query)
            {
                walk.Visited[nextRow, nextCol] = true;
                walk.Stack.Push((nextRow, nextCol));
            }
        }
    }

    private readonly record struct FloodWalk(
        int[][] Grid,
        bool[,] Visited,
        Stack<(int Row, int Col)> Stack,
        int Query);

    // Composed: one shared flood fill for all queries. Sorting the queries ascending
    // and draining a min-heap frontier ordered by cell value - Heap<Element,TOrder>
    // closed over ByPriorityOrder<GridNode,int>, the same frontier shape
    // ShortestPath.Dijkstra uses - visits cells in exactly the order they would first
    // become reachable at an increasing threshold, so each query's answer is just how
    // many cells the heap has popped once its smallest remaining value stops being
    // below that query. O(Rows*Cols*log(Rows*Cols) + QueryCount*log QueryCount) in
    // total, however many queries there are.
    //
    // Neighbor iteration reuses Graph.Grids' GridNode/GridChildren/GridTopology as
    // pure 4-directional bounds arithmetic over an all-passable Grid - the value
    // threshold, not a grid obstacle, is what gates movement here, so nothing needs
    // an obstacle-aware Grid (the same move FindTheSafestPathInAGridSolution makes) -
    // with a HashSet<GridNode> tracking visited cells the way
    // ShortestPath.SearchState.Settled does.
    public static int[] MaxPointsByMinHeapFrontier(int[][] grid, int[] queries)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        var start = new GridNode(0, 0, new Grid(AllPassable(rows, cols)));
        var walk = new HeapWalk(new CellFrontier(), new HashSet<GridNode> { start }, grid);
        walk.Frontier.Push((start, grid[0][0]));

        var order = queries
            .Select((value, index) => (Value: value, Index: index))
            .OrderBy(query => query.Value);

        var answers = new int[queries.Length];
        var points = 0;

        foreach (var query in order)
        {
            points = AdvanceFrontier(walk, query.Value, points);
            answers[query.Index] = points;
        }

        return answers;
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

    private static int AdvanceFrontier(HeapWalk walk, int query, int points)
    {
        while (walk.Frontier.TryPeek(out var top) && top.Priority < query)
        {
            walk.Frontier.TryPop(out var current);
            points++;

            var children = GridTopology.GetChildren(current.Node);

            for (var i = 0; i < children.Count; i++)
            {
                var neighbor = children.Get(i);

                if (walk.Visited.Add(neighbor))
                {
                    walk.Frontier.Push((neighbor, walk.Grid[neighbor.Row][neighbor.Col]));
                }
            }
        }

        return points;
    }

    private readonly record struct HeapWalk(CellFrontier Frontier, HashSet<GridNode> Visited, int[][] Grid);
}
