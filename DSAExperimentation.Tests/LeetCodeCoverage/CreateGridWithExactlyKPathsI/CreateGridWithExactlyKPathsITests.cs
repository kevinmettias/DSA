using DSAExperimentation.LeetCode.CreateGridWithExactlyKPathsI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CreateGridWithExactlyKPathsI;

// Harness only. Grid construction is CreateGridWithExactlyKPathsISolution's; this
// file pins both strategies to LeetCode's published examples plus a
// count-the-paths check, since "return any grid" means the published output
// string is not itself the oracle - a DP walk over the returned grid is.
public sealed class CreateGridWithExactlyKPathsITests
{
    public static TheoryData<int, int, int, bool> Examples =>
        new()
        {
            { 2, 3, 2, true },
            { 3, 3, 4, true },
            { 1, 4, 2, false }, // No grid exists; the expected output is [].
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateGridByPathCountDp_LeetCodeExamples_ReturnsGridWithExactlyKPaths(
        int m, int n, int k, bool expectExists) =>
        AssertGrid(CreateGridWithExactlyKPathsISolution.CreateGridByPathCountDp(m, n, k), m, n, k, expectExists);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateGridByBinomialFormula_LeetCodeExamples_ReturnsGridWithExactlyKPaths(
        int m, int n, int k, bool expectExists) =>
        AssertGrid(CreateGridWithExactlyKPathsISolution.CreateGridByBinomialFormula(m, n, k), m, n, k, expectExists);

    private static void AssertGrid(string[] grid, int m, int n, int k, bool expectExists)
    {
        if (!expectExists)
        {
            Assert.Empty(grid);
            return;
        }

        Assert.Equal(m, grid.Length);
        Assert.All(grid, row => Assert.Equal(n, row.Length));
        Assert.Equal('.', grid[0][0]);
        Assert.Equal('.', grid[m - 1][n - 1]);
        Assert.Equal(k, CountPaths(grid));
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

                dp[i, j] = i == 0 && j == 0
                    ? 1
                    : (i > 0 ? dp[i - 1, j] : 0) + (j > 0 ? dp[i, j - 1] : 0);
            }
        }

        return dp[m - 1, n - 1];
    }
}
