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
                if (grid[r][c] != 1)
                {
                    continue;
                }

                var island = DepthFirstSearch.Traverse((r, c), Neighbors);
                best = Math.Max(best, island.Count);

                foreach (var (row, col) in island)
                {
                    grid[row][col] = 0;
                }
            }
        }

        return best;

        IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p)
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = p.Row + dRow;
                var nextCol = p.Col + dCol;

                if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
                {
                    continue;
                }

                if (grid[nextRow][nextCol] != 1)
                {
                    continue;
                }

                yield return (nextRow, nextCol);
            }
        }
    }
}
