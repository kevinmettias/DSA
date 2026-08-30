using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathWithMinimumEffort;

// LeetCode 1631. Path With Minimum Effort: the same minimax-Dijkstra frontier
// SwimInRisingWaterTests uses - this repo's own Heap<Element,TOrder> ordered by
// ByPriorityOrder<TNode,TWeight>, over (row, col) cells keyed by the bottleneck
// (max) absolute height difference crossed so far, Math.Max standing in for +.
// Unlike Swim in Rising Water's per-cell elevation - a value fixed by the
// destination cell alone, which lets that solution mark a cell visited the
// moment it's pushed - here the edge weight depends on BOTH endpoints, so a
// cheaper route can still reach an already-pushed cell later. Settling
// happens at pop time instead, the same "first pop is final" rule
// ShortestPath.cs's own Settled set relies on, plus a bestEffort table so a
// cell is only re-pushed when a strictly smaller candidate is found.
public sealed partial class PathWithMinimumEffortTests
{
    [Fact]
    public void MinimumEffortPath_LeetCodeExampleOne_ReturnsBottleneckAlongCheapestRoute()
    {
        int[][] heights =
        [
            [1, 2, 2],
            [3, 8, 2],
            [5, 3, 5],
        ];

        Assert.Equal(2, MinimumEffortPath(heights));
    }

    [Fact]
    public void MinimumEffortPath_LeetCodeExampleTwo_ReturnsBottleneckAlongCheapestRoute()
    {
        int[][] heights =
        [
            [1, 2, 3],
            [3, 8, 4],
            [5, 3, 5],
        ];

        Assert.Equal(1, MinimumEffortPath(heights));
    }

    [Fact]
    public void MinimumEffortPath_SingleCellGrid_ReturnsZero()
    {
        int[][] heights = [[5]];

        Assert.Equal(0, MinimumEffortPath(heights));
    }

    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    private static int MinimumEffortPath(int[][] heights)
    {
        var rows = heights.Length;
        var cols = heights[0].Length;
        var settled = new bool[rows, cols];
        var bestEffort = new int[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                bestEffort[r, c] = int.MaxValue;
            }
        }

        bestEffort[0, 0] = 0;
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();
        frontier.Push(((0, 0), 0));

        while (frontier.TryPop(out var entry))
        {
            var (row, col) = entry.Node;

            if (settled[row, col])
            {
                continue;
            }

            settled[row, col] = true;
            var effort = entry.Priority;

            if (row == rows - 1 && col == cols - 1)
            {
                return effort;
            }

            foreach (var (dr, dc) in Directions)
            {
                var nr = row + dr;
                var nc = col + dc;

                if (nr < 0 || nr >= rows || nc < 0 || nc >= cols || settled[nr, nc])
                {
                    continue;
                }

                var candidate = Math.Max(effort, Math.Abs(heights[nr][nc] - heights[row][col]));

                if (candidate < bestEffort[nr, nc])
                {
                    bestEffort[nr, nc] = candidate;
                    frontier.Push(((nr, nc), candidate));
                }
            }
        }

        return 0;
    }
}
