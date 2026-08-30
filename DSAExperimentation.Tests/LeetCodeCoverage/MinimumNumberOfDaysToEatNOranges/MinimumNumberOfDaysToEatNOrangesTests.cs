using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfDaysToEatNOranges;

// LeetCode 1553. Minimum Number of Days to Eat N Oranges: this repo's own
// Memoizer-driven DP recurrence (same IntegerReplacementTests/FibonacciNumberTests
// composition) over days(n) = n when n <= 1, else
// 1 + min(n % 2 + days(n / 2), n % 3 + days(n / 3)) - eat off the remainder one at a
// time, then spend a single day halving or thirding whatever is left. n stays well
// inside Int32 even at the problem's stated 2*10^9 upper bound (below
// int.MaxValue), so unlike IntegerReplacement no long widening is needed here.
public sealed partial class MinimumNumberOfDaysToEatNOrangesTests
{
    [Theory]
    [InlineData(10, 4)]
    [InlineData(6, 3)]
    [InlineData(1, 1)]
    [InlineData(56, 6)]
    public void MinDays_LeetCodeExamples_ReturnsMinimumDayCount(int n, int expected)
        => Assert.Equal(expected, MinDays(n));

    private static int MinDays(int n)
        => Memoizer.Memoize<int, int>(n, (value, days) => value <= 1
            ? value
            : 1 + Math.Min((value % 2) + days(value / 2), (value % 3) + days(value / 3)));
}
