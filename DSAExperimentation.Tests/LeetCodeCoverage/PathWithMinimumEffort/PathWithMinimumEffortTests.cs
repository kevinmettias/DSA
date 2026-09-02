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
        var search = new EffortSearch(heights, rows, cols);

        return search.Run();
    }

    private sealed class EffortSearch(int[][] heights, int rows, int cols)
    {
        private readonly bool[,] _settled = new bool[rows, cols];
        private readonly int[,] _bestEffort = BuildBestEffort(rows, cols);
        private readonly Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> _frontier = new();

        private static int[,] BuildBestEffort(int rows, int cols)
        {
            var bestEffort = new int[rows, cols];

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    bestEffort[r, c] = int.MaxValue;
                }
            }

            return bestEffort;
        }

        public int Run()
        {
            _bestEffort[0, 0] = 0;
            _frontier.Push(((0, 0), 0));

            while (_frontier.TryPop(out var entry))
            {
                var result = ProcessEntry(entry);

                if (result is not null)
                {
                    return result.Value;
                }
            }

            return 0;
        }

        private int? ProcessEntry(((int Row, int Col) Node, int Priority) entry)
        {
            var (row, col) = entry.Node;

            if (_settled[row, col])
            {
                return null;
            }

            _settled[row, col] = true;
            var effort = entry.Priority;

            if (row == rows - 1 && col == cols - 1)
            {
                return effort;
            }

            RelaxNeighbors(row, col, effort);
            return null;
        }

        private void RelaxNeighbors(int row, int col, int effort)
        {
            foreach (var direction in Directions)
            {
                RelaxNeighbor(row, col, effort, direction);
            }
        }

        private void RelaxNeighbor(int row, int col, int effort, (int Row, int Col) direction)
        {
            var nr = row + direction.Row;
            var nc = col + direction.Col;

            if (nr < 0 || nr >= rows || nc < 0 || nc >= cols || _settled[nr, nc])
            {
                return;
            }

            var candidate = Math.Max(effort, Math.Abs(heights[nr][nc] - heights[row][col]));

            if (candidate < _bestEffort[nr, nc])
            {
                _bestEffort[nr, nc] = candidate;
                _frontier.Push(((nr, nc), candidate));
            }
        }
    }
}
