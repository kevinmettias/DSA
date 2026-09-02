using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SwimInRisingWater;

// LeetCode 778. Swim in Rising Water: the same Dijkstra-shaped grid frontier
// TrappingRainWaterIITests uses - this repo's own Heap<Element,TOrder>, ordered
// by ByPriorityOrder<TNode,TWeight>, over (row, col) cells keyed by the minimax
// (bottleneck) cost of the path reached so far, instead of a summed distance.
// Popping the globally cheapest frontier cell first guarantees each cell is
// first reached at the true minimum "max elevation crossed" cost - the same
// settle-once argument Dijkstra's own first-pop-is-final proof relies on, just
// with Math.Max standing in for +.
public sealed partial class SwimInRisingWaterTests
{
    [Fact]
    public void SwimInWater_TwoByTwoGrid_ReturnsMinimumPossibleTime()
    {
        int[][] grid =
        [
            [0, 2],
            [1, 3],
        ];

        Assert.Equal(3, SwimInWater(grid));
    }

    [Fact]
    public void SwimInWater_FiveByFiveGrid_ReturnsMinimumPossibleTime()
    {
        int[][] grid =
        [
            [0, 1, 2, 3, 4],
            [24, 23, 22, 21, 5],
            [12, 13, 14, 15, 16],
            [11, 17, 18, 19, 20],
            [10, 9, 8, 7, 6],
        ];

        Assert.Equal(16, SwimInWater(grid));
    }

    [Fact]
    public void SwimInWater_SingleCellGrid_ReturnsThatCellsElevation()
    {
        int[][] grid = [[0]];

        Assert.Equal(0, SwimInWater(grid));
    }

    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    private static int SwimInWater(int[][] grid)
    {
        var n = grid.Length;
        var visited = new bool[n, n];
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();

        frontier.Push(((0, 0), grid[0][0]));
        visited[0, 0] = true;

        while (frontier.TryPop(out var entry))
        {
            var (row, col) = entry.Node;
            var time = entry.Priority;

            if (row == n - 1 && col == n - 1)
            {
                return time;
            }

            ExploreNeighbors(entry, grid, visited, frontier);
        }

        throw new InvalidOperationException("Unreachable for a valid n x n grid: every cell connects to (0,0).");
    }

    private static void ExploreNeighbors(
        ((int Row, int Col) Node, int Priority) entry,
        int[][] grid,
        bool[,] visited,
        Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> frontier)
    {
        var (row, col) = entry.Node;
        var time = entry.Priority;
        var n = grid.Length;

        foreach (var (dr, dc) in Directions)
        {
            var nr = row + dr;
            var nc = col + dc;

            if (nr < 0 || nr >= n || nc < 0 || nc >= n || visited[nr, nc])
            {
                continue;
            }

            visited[nr, nc] = true;
            frontier.Push(((nr, nc), Math.Max(time, grid[nr][nc])));
        }
    }
}
