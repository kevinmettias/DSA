using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfFishInAGrid;

// LeetCode 2658. Maximum Number of Fish in a Grid: the same border-agnostic
// flood-fill shape MaxAreaOfIslandTests/NumberOfIslandsTests already use for a
// binary land/water grid - a water cell here is any grid[r][c] > 0 (land is
// exactly 0), so this repo's own DepthFirstSearch.Traverse walks one connected
// water component per unvisited water cell, and instead of the traversal's own
// reachable-node count (MaxAreaOfIsland's area), the answer sums each visited
// cell's own fish count.
public sealed partial class MaximumNumberOfFishInAGridTests
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Fact]
    public void FindMaxFish_ClassicExample_ReturnsLargestComponentFishTotal()
    {
        int[][] grid =
        [
            [0, 2, 1, 0],
            [4, 0, 0, 3],
            [1, 0, 0, 4],
            [0, 3, 2, 0],
        ];

        Assert.Equal(7, FindMaxFish(grid));
    }

    [Fact]
    public void FindMaxFish_AllLand_ReturnsZero()
    {
        int[][] grid = [[0, 0], [0, 0]];

        Assert.Equal(0, FindMaxFish(grid));
    }

    private static int FindMaxFish(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var total = FloodFillFishTotal(r, c, grid);
                best = Math.Max(best, total);
            }
        }

        return best;
    }

    private static int FloodFillFishTotal(int r, int c, int[][] grid)
    {
        if (grid[r][c] == 0)
        {
            return 0;
        }

        var component = DepthFirstSearch.Traverse((Row: r, Col: c), p => Neighbors(p, grid));
        var total = component.Sum(p => grid[p.Row][p.Col]);

        foreach (var (row, col) in component)
        {
            grid[row][col] = 0;
        }

        return total;
    }

    private static IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p, int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: p.Row + dRow, Col: p.Col + dCol);

            if (IsWater(next, rows, cols, grid))
            {
                yield return next;
            }
        }
    }

    private static bool IsWater((int Row, int Col) next, int rows, int cols, int[][] grid)
    {
        if (next.Row < 0 || next.Row >= rows || next.Col < 0 || next.Col >= cols)
        {
            return false;
        }

        return grid[next.Row][next.Col] > 0;
    }
}
