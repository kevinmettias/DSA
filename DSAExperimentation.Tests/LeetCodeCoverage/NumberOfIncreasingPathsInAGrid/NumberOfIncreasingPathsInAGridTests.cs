using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfIncreasingPathsInAGrid;

// LeetCode 2328. Number of Increasing Paths in a Grid: the same Memoizer-over-
// (Row,Col)-state recurrence LongestIncreasingPathInAMatrixTests already establishes
// for this repo - a strictly-increasing-value relation can never cycle back to a
// cell already on the path, so the induced relation is a DAG and Memoizer's
// well-founded-state precondition never trips - just counting every increasing path
// starting at each cell (1 for the trivial single-cell path, plus the count of paths
// from each strictly greater neighbor) instead of taking the longest one, summed
// modulo 1e9+7 over every cell as a candidate start.
public sealed class NumberOfIncreasingPathsInAGridTests
{
    private const long Mod = 1_000_000_007L;
    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Fact]
    public void CountPaths_TwoByTwoExample_ReturnsEight()
    {
        int[][] grid = [[1, 1], [3, 4]];

        Assert.Equal(8, CountPaths(grid));
    }

    [Fact]
    public void CountPaths_TwoRowsOneColumnExample_ReturnsThree()
    {
        int[][] grid = [[1], [2]];

        Assert.Equal(3, CountPaths(grid));
    }

    [Fact]
    public void CountPaths_SingleCell_ReturnsOne()
    {
        int[][] grid = [[5]];

        Assert.Equal(1, CountPaths(grid));
    }

    private readonly record struct PathsGrid(int[][] Values, int Rows, int Cols);

    private static int CountPaths(int[][] grid)
    {
        var pathsGrid = new PathsGrid(grid, grid.Length, grid[0].Length);
        var total = 0L;

        for (var row = 0; row < pathsGrid.Rows; row++)
        {
            for (var col = 0; col < pathsGrid.Cols; col++)
            {
                total += Memoizer.Memoize<(int Row, int Col), long>(
                    (row, col), (state, pathsFrom) => PathsFrom(pathsGrid, state, pathsFrom));
                total %= Mod;
            }
        }

        return (int)total;
    }

    private static long PathsFrom(PathsGrid grid, (int Row, int Col) state, Func<(int Row, int Col), long> pathsFrom)
    {
        var count = 1L;

        foreach (var (rowOffset, colOffset) in Directions)
        {
            var nextRow = state.Row + rowOffset;
            var nextCol = state.Col + colOffset;

            if (nextRow >= 0 && nextRow < grid.Rows && nextCol >= 0 && nextCol < grid.Cols
                && grid.Values[nextRow][nextCol] > grid.Values[state.Row][state.Col])
            {
                count += pathsFrom((nextRow, nextCol));
            }
        }

        return count % Mod;
    }
}
