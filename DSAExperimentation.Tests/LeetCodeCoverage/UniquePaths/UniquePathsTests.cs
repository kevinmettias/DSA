using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UniquePaths;

// LeetCode 62. Unique Paths: Memoizer caches the grid recurrence from each cell
// to the bottom-right destination.
public sealed partial class UniquePathsTests
{
    [Theory]
    [InlineData(3, 7, 28)]
    [InlineData(3, 2, 3)]
    public void CountPaths_LeetCodeExamples_ReturnsExpectedCount(int m, int n, int expected)
        => Assert.Equal(expected, CountPaths(m, n));

    private static int CountPaths(int m, int n)
    {
        return Memoizer.Memoize<(int Row, int Col), int>((0, 0), WaysFrom);

        int WaysFrom((int Row, int Col) state, Func<(int Row, int Col), int> ways)
        {
            var (row, col) = state;
            if (row == m - 1 && col == n - 1) return 1;
            var total = 0;
            if (row + 1 < m) total += ways((row + 1, col));
            if (col + 1 < n) total += ways((row, col + 1));
            return total;
        }
    }
}
