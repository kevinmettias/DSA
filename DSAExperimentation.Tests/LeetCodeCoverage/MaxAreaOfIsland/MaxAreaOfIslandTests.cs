using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxAreaOfIsland;

// LeetCode 695. Max Area of Island: the same border-agnostic flood-fill shape
// NumberOfIslandsTests/SurroundedRegionsTests/PacificAtlanticWaterFlowTests already
// use - this repo's own DepthFirstSearch.Traverse walks one island's full land
// component from each unvisited land cell, and the traversal's own reachable-node
// count (instead of just "component found, increment a counter") is the island's
// area. Traversed cells are zeroed in place afterward so the outer scan's own
// "still land?" check doubles as the visited set, the same in-place-mutation trick
// NumberOfIslandsTests uses.
public sealed partial class MaxAreaOfIslandTests
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Fact]
    public void MaxAreaOfIsland_ClassicExample_ReturnsLargestIslandArea()
    {
        int[][] grid =
        [
            [0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0],
            [0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0],
            [0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0],
            [0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0],
            [0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0],
            [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0],
            [0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0],
            [0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0],
        ];

        Assert.Equal(6, MaxAreaOfIsland(grid));
    }

    [Fact]
    public void MaxAreaOfIsland_NoLand_ReturnsZero()
    {
        int[][] grid = [[0, 0], [0, 0]];

        Assert.Equal(0, MaxAreaOfIsland(grid));
    }

    private static int MaxAreaOfIsland(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var area = FloodFillIslandArea(r, c, grid);
                best = Math.Max(best, area);
            }
        }

        return best;
    }

    private static int FloodFillIslandArea(int r, int c, int[][] grid)
    {
        if (grid[r][c] != 1)
        {
            return 0;
        }

        var island = DepthFirstSearch.Traverse((r, c), p => Neighbors(p, grid));

        foreach (var (row, col) in island)
        {
            grid[row][col] = 0;
        }

        return island.Count;
    }

    private static IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p, int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: p.Row + dRow, Col: p.Col + dCol);

            if (IsLand(next, rows, cols, grid))
            {
                yield return next;
            }
        }
    }

    private static bool IsLand((int Row, int Col) next, int rows, int cols, int[][] grid)
    {
        if (next.Row < 0 || next.Row >= rows || next.Col < 0 || next.Col >= cols)
        {
            return false;
        }

        return grid[next.Row][next.Col] == 1;
    }
}
