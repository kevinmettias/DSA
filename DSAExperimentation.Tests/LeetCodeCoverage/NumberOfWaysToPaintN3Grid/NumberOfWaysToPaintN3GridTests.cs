using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToPaintN3Grid;

// LeetCode 1411. Number of Ways to Paint N x 3 Grid: each row's coloring is
// either "Same" (its two end cells match - an ABA shape, 6 colorings) or
// "Different" (all three cells distinct - an ABC shape, 6 colorings); a
// compatible next row contributes 3 (Same) or 2 (Different) ways following a
// Same row, and 2 (Same) or 2 (Different) ways following a Different row. The
// coupled pair (same[row], different[row]) is exactly the small-fixed-state
// recurrence this repo's own Memoizer<TState,TResult> already expresses for
// FibonacciNumberTests/NthTribonacciNumberTests - here TResult is the
// (Same, Different) pair itself instead of a single int.
public sealed partial class NumberOfWaysToPaintN3GridTests
{
    private const long Modulus = 1_000_000_007;

    [Fact]
    public void NumOfWays_SingleRow_ReturnsAllTwelveColorings()
        => Assert.Equal(12, NumOfWays(1));

    [Theory]
    [InlineData(2, 54)]
    [InlineData(3, 246)]
    public void NumOfWays_FewRows_MatchesKnownRowCounts(int n, long expected)
        => Assert.Equal(expected, NumOfWays(n));

    [Fact]
    public void NumOfWays_LargeInput_MatchesModularResult()
        => Assert.Equal(748221310, NumOfWays(300));

    private static long NumOfWays(int n)
    {
        var (same, different) = Memoizer.Memoize<int, (long Same, long Different)>(n, Ways);
        return (same + different) % Modulus;
    }

    private static (long Same, long Different) Ways(int row, Func<int, (long Same, long Different)> ways)
    {
        if (row == 1)
        {
            return (6, 6);
        }

        var (prevSame, prevDifferent) = ways(row - 1);

        return (
            (3 * prevSame + 2 * prevDifferent) % Modulus,
            (2 * prevSame + 2 * prevDifferent) % Modulus);
    }
}
