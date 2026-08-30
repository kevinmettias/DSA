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
                    Sink(r, c);
                }
            }
        }

        var enclaves = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] == 1)
                {
                    enclaves++;
                }
            }
        }

        return enclaves;

        void Sink(int startRow, int startCol)
        {
            if (grid[startRow][startCol] != 1)
            {
                return;
            }

            var component = DepthFirstSearch.Traverse((startRow, startCol), Neighbors);

            foreach (var (row, col) in component)
            {
                grid[row][col] = 0;
            }
        }

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
