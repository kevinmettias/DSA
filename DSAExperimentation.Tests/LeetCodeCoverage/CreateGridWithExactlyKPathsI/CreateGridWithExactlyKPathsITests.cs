using DSAExperimentation.LeetCode.CreateGridWithExactlyKPathsI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CreateGridWithExactlyKPathsI;

// Harness only. Grid construction is CreateGridWithExactlyKPathsISolution's; this
// file pins both strategies to LeetCode's published examples plus a
// count-the-paths check, since "return any grid" means the published output
// string is not itself the oracle - a DP walk over the returned grid is.
public sealed class CreateGridWithExactlyKPathsITests
{
    public static TheoryData<GridPathExample> Examples =>
        new()
        {
            { new GridPathExample(M: 2, N: 3, K: 2, ExpectExists: true) },
            { new GridPathExample(M: 3, N: 3, K: 4, ExpectExists: true) },
            // No grid exists; the expected output is [].
            { new GridPathExample(M: 1, N: 4, K: 2, ExpectExists: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateGridByPathCountDp_LeetCodeExamples_ReturnsGridWithExactlyKPaths(
        GridPathExample example)
    {
        var grid = CreateGridWithExactlyKPathsISolution.CreateGridByPathCountDp(
            example.M, example.N, example.K);

        AssertGrid(grid, example);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateGridByBinomialFormula_LeetCodeExamples_ReturnsGridWithExactlyKPaths(
        GridPathExample example)
    {
        var grid = CreateGridWithExactlyKPathsISolution.CreateGridByBinomialFormula(
            example.M, example.N, example.K);

        AssertGrid(grid, example);
    }

    private static void AssertGrid(string[] grid, GridPathExample example)
    {
        if (!example.ExpectExists)
        {
            Assert.Empty(grid);
            return;
        }

        Assert.Equal(example.M, grid.Length);
        Assert.All(grid, row => Assert.Equal(example.N, row.Length));
        Assert.Equal('.', grid[0][0]);
        Assert.Equal('.', grid[example.M - 1][example.N - 1]);
        Assert.Equal(example.K, CountPaths(grid));
    }

    // The independent oracle every returned grid is checked against: plain
    // unique-paths DP over whatever obstacle layout the strategy chose.
    private static int CountPaths(string[] grid)
    {
        var m = grid.Length;
        var n = grid[0].Length;
        var dp = new int[m, n];

        for (var i = 0; i < m; i++)
        {
            for (var j = 0; j < n; j++)
            {
                if (grid[i][j] == '#')
                {
                    dp[i, j] = 0;
                    continue;
                }

                dp[i, j] = PathsTo(i, j, dp);
            }
        }

        return dp[m - 1, n - 1];
    }

    // The start cell holds the one path that exists before any step; every other cell
    // sums what arrives from above and from the left.
    private static int PathsTo(int row, int column, int[,] pathsSoFar)
    {
        if (row == 0 && column == 0)
        {
            return 1;
        }

        return PathsFromAbove(row, column, pathsSoFar) + PathsFromLeft(row, column, pathsSoFar);
    }

    // A cell in the top row has nothing above it, which contributes no paths.
    private static int PathsFromAbove(int row, int column, int[,] pathsSoFar)
    {
        if (row == 0)
        {
            return 0;
        }

        return pathsSoFar[row - 1, column];
    }

    // A cell in the left column has nothing to its left, which contributes no paths.
    private static int PathsFromLeft(int row, int column, int[,] pathsSoFar)
    {
        if (column == 0)
        {
            return 0;
        }

        return pathsSoFar[row, column - 1];
    }

    // One LeetCode example: the grid dimensions, the path count the returned grid must
    // produce, and whether any grid exists for those dimensions at all. The four travel
    // together into every assertion, so they are named here rather than left as four
    // positional arguments a transposed call site would accept in silence.
    public readonly record struct GridPathExample(int M, int N, int K, bool ExpectExists);
}
