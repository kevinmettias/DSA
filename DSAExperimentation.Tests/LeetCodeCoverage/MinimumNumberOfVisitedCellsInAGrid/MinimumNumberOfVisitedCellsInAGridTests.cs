using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfVisitedCellsInAGrid.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfVisitedCellsInAGrid;

// LeetCode 2617. Minimum Number of Visited Cells in a Grid: from (row, col) with
// stored value v, you may jump to any of (row, col+1..col+v) or (row+1..row+v, col).
// The fewest cells to reach the bottom-right corner is exactly a shortest-path query
// over the implicit graph where cells are nodes and one jump is one edge -
// Reduce.Graph's own BreadthFirstReduceOrder + DistanceMapReduceAlgebra already
// answers "distance from a root to every node" (the same composition
// WordLadderSolution/OpenTheLockSolution use), so this problem reduces to supplying
// the right IGraphTopology: JumpGridTopology, whose JumpGridChildren computes each
// cell's reachable jumps directly from its stored value instead of scanning the
// whole row/column. LeetCode counts cells visited, not edges walked, so every
// strategy here answers distance + 1.
public sealed partial class MinimumNumberOfVisitedCellsInAGridTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[3, 4, 2, 1], [4, 2, 3, 1], [2, 1, 0, 0], [2, 4, 0, 0]], 4 },
            { [[3, 4, 2, 1], [4, 2, 1, 1], [2, 1, 1, 0], [3, 4, 1, 0]], 3 },
            { [[2, 1, 0], [1, 0, 0]], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinVisitedCellsByBruteForceScan_LeetCodeExamples_ReturnsShortestPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MinVisitedCellsByBruteForceScan(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinVisitedCellsByReduceGraph_LeetCodeExamples_ReturnsShortestPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MinVisitedCellsByReduceGraph(grid));

    // Baseline: no on-demand jump arithmetic - for every dequeued cell, the entire
    // rest of its row and its entire column are scanned (O(rows + cols) per pop),
    // the O(v)-per-pop primitive-composed arm below has to beat.
    private static int MinVisitedCellsByBruteForceScan(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var distances = new int[rows, cols];

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                distances[row, col] = -1;
            }
        }

        distances[0, 0] = 0;

        var queue = new Queue<(int Row, int Col)>();
        queue.Enqueue((0, 0));

        while (queue.Count > 0)
        {
            var (row, col) = queue.Dequeue();
            var jump = grid[row][col];

            for (var nextCol = 0; nextCol < cols; nextCol++)
            {
                if (nextCol > col && nextCol <= col + jump && distances[row, nextCol] == -1)
                {
                    distances[row, nextCol] = distances[row, col] + 1;
                    queue.Enqueue((row, nextCol));
                }
            }

            for (var nextRow = 0; nextRow < rows; nextRow++)
            {
                if (nextRow > row && nextRow <= row + jump && distances[nextRow, col] == -1)
                {
                    distances[nextRow, col] = distances[row, col] + 1;
                    queue.Enqueue((nextRow, col));
                }
            }
        }

        var target = distances[rows - 1, cols - 1];
        return target == -1 ? -1 : target + 1;
    }

    // This repo's own Reduce.Graph: JumpGridChildren only ever produces the cells
    // actually reachable in one jump, so each pop does O(v) work instead of the
    // O(rows + cols) scan MinVisitedCellsByBruteForceScan needs.
    private static int MinVisitedCellsByReduceGraph(int[][] grid)
    {
        var jumpGrid = new JumpGrid(grid);
        var source = new JumpGridNode(0, 0, jumpGrid);
        var target = new JumpGridNode(jumpGrid.Rows - 1, jumpGrid.Cols - 1, jumpGrid);

        var distanceByNode = Reduce.Graph<
            JumpGridNode, JumpGridTopology, JumpGridChildren,
            NaturalChildOrder<JumpGridNode, JumpGridChildren>, JumpGridChildren,
            BreadthFirstReduceOrder<JumpGridNode>,
            DistanceMapReduceAlgebra<JumpGridNode>, Dictionary<JumpGridNode, int>>(source);

        return distanceByNode.TryGetValue(target, out var distance) ? distance + 1 : -1;
    }
}
