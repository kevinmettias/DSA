using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.MinimumNumberOfVisitedCellsInAGrid;

// LeetCode 2617. Minimum Number of Visited Cells in a Grid: from (row, col) with
// stored value v, you may jump to any of (row, col+1..col+v) or (row+1..row+v, col).
// The fewest cells to reach the bottom-right corner is exactly a shortest-path query
// over the implicit graph where cells are nodes and one jump is one edge -
// Reduce.Graph's own BreadthFirstReduceOrder + DistanceMapReduceAlgebra already
// answers "distance from a root to every node" (the same composition
// OpenTheLockSolution uses), so this problem reduces to supplying the right
// IGraphTopology: JumpGridTopology, whose JumpGridChildren computes each cell's
// reachable jumps directly from its stored value instead of scanning the whole
// row/column. LeetCode counts cells visited, not edges walked, so every strategy
// here answers distance + 1.
internal static class MinimumNumberOfVisitedCellsInAGridSolution
{
    // Baseline: no on-demand jump arithmetic - for every dequeued cell, the entire
    // rest of its row and its entire column are scanned (O(rows + cols) per pop),
    // which is the cost the primitive-composed arm below has to beat. Deliberately
    // written with a BCL Queue and a plain int[,] only: it is what you would write
    // without this repo.
    public static int MinVisitedCellsByBruteForceScan(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var distances = CreateUnvisitedDistanceGrid(rows, cols);

        var queue = new Queue<(int Row, int Col)>();
        queue.Enqueue((0, 0));

        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            RelaxJumpsFrom(grid, distances, queue, cell);
        }

        var target = distances[rows - 1, cols - 1];

        return target == LeetCodeAnswer.None ? LeetCodeAnswer.None : CellsVisitedFor(target);
    }

    // "Not yet reached" is the answer sentinel itself, so a freshly allocated grid is
    // already the unvisited state; only the start cell has to be seeded.
    private static int[,] CreateUnvisitedDistanceGrid(int rows, int cols)
    {
        var distances = new int[rows, cols];

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                distances[row, col] = LeetCodeAnswer.None;
            }
        }

        distances[0, 0] = 0;

        return distances;
    }

    // The whole rest of the popped cell's row is scanned, then its whole column, each
    // relaxing only the cells the stored value reaches and has not reached yet.
    private static void RelaxJumpsFrom(
        int[][] grid, int[,] distances, Queue<(int Row, int Col)> queue, (int Row, int Col) cell)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var (row, col) = cell;
        var jump = grid[row][col];

        for (var nextCol = 0; nextCol < cols; nextCol++)
        {
            if (IsUnvisitedJumpTarget(nextCol, col, jump, distances[row, nextCol]))
            {
                distances[row, nextCol] = distances[row, col] + 1;
                queue.Enqueue((row, nextCol));
            }
        }

        for (var nextRow = 0; nextRow < rows; nextRow++)
        {
            if (IsUnvisitedJumpTarget(nextRow, row, jump, distances[nextRow, col]))
            {
                distances[nextRow, col] = distances[row, col] + 1;
                queue.Enqueue((nextRow, col));
            }
        }
    }

    // A cell is worth a jump when it lies ahead along the line, no further than the
    // stored value allows, and the walk has not reached it yet.
    private static bool IsUnvisitedJumpTarget(int index, int from, int jump, int distance)
        => index > from && index <= from + jump && distance == LeetCodeAnswer.None;

    public static int MinVisitedCellsByReduceGraph(int[][] grid)
        => MinVisitedCellsByReduceGraph(new JumpGrid(grid));

    // This repo's own Reduce.Graph: JumpGridChildren only ever produces the cells
    // actually reachable in one jump, so each pop does O(v) work instead of the
    // O(rows + cols) scan MinVisitedCellsByBruteForceScan needs.
    //
    // #17.4's hoisted overload: JumpGrid is not IEnumerable, so this can never be
    // confused with the LeetCode-shaped overload above, and a benchmark can charge
    // grid construction to [GlobalSetup].
    public static int MinVisitedCellsByReduceGraph(JumpGrid grid)
    {
        var source = new JumpGridNode(0, 0, grid);
        var target = new JumpGridNode(grid.Rows - 1, grid.Cols - 1, grid);

        var distanceByNode = Reduce.Graph<
            JumpGridNode, JumpGridTopology, JumpGridChildren,
            NaturalChildOrder<JumpGridNode, JumpGridChildren>, JumpGridChildren,
            BreadthFirstReduceOrder<JumpGridNode>,
            DistanceMapReduceAlgebra<JumpGridNode>, Dictionary<JumpGridNode, int>>(source);

        return distanceByNode.TryGetValue(target, out var distance) ? CellsVisitedFor(distance) : LeetCodeAnswer.None;
    }

    // LeetCode counts the cells walked through, not the edges between them, so every
    // strategy here answers its search distance plus the starting cell.
    private static int CellsVisitedFor(int edgeDistance) => edgeDistance + 1;
}
