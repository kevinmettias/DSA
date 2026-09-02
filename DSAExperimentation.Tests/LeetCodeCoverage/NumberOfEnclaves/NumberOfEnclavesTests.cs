using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfEnclaves;

// LeetCode 1020. Number of Enclaves: the same border-flood-fill shape
// MaxAreaOfIslandTests/NumberOfIslandsTests use - this repo's own
// DepthFirstSearch.Traverse floods every land cell reachable from the grid's
// border (which can therefore always walk off the grid, so it's never an
// enclave), zeroing each one out in place. Whatever land remains afterward is,
// by definition, land that can never reach the boundary - an enclave - and a
// final scan just counts it.
public sealed partial class NumberOfEnclavesTests
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Fact]
    public void NumEnclaves_ClassicExample_ReturnsThree()
    {
        int[][] grid =
        [
            [0, 0, 0, 0],
            [1, 0, 1, 0],
            [0, 1, 1, 0],
            [0, 0, 0, 0],
        ];

        Assert.Equal(3, NumEnclaves(grid));
    }

    [Fact]
    public void NumEnclaves_AllLandTouchesBorder_ReturnsZero()
    {
        int[][] grid =
        [
            [0, 1, 1, 0],
            [0, 0, 1, 0],
            [1, 0, 0, 0],
            [0, 1, 1, 0],
        ];

        Assert.Equal(0, NumEnclaves(grid));
    }

    private static int NumEnclaves(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var onBorder = r == 0 || r == rows - 1 || c == 0 || c == cols - 1;

                if (onBorder)
                {
                    Sink(r, c, grid);
                }
            }
        }

        return CountRemainingLand(grid);
    }

    private static int CountRemainingLand(int[][] grid)
    {
        var enclaves = 0;

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] == 1)
                {
                    enclaves++;
                }
            }
        }

        return enclaves;
    }

    private static void Sink(int startRow, int startCol, int[][] grid)
    {
        if (grid[startRow][startCol] != 1)
        {
            return;
        }

        var component = DepthFirstSearch.Traverse((startRow, startCol), p => Neighbors(p, grid));

        foreach (var (row, col) in component)
        {
            grid[row][col] = 0;
        }
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
