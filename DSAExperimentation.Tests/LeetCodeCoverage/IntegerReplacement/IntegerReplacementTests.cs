using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IntegerReplacement;

// LeetCode 397. Integer Replacement: this repo's own Memoizer-driven DP recurrence
// (same CountingBitsTests/HouseRobberTests composition) over
// replace(n) = 0 when n == 1, 1 + replace(n/2) when n is even, and
// 1 + min(replace(n-1), replace(n+1)) when n is odd. TState is long, not int,
// specifically because n can be int.MaxValue (2^31 - 1) and the n+1 branch would
// otherwise silently overflow Int32.
public sealed partial class IntegerReplacementTests
{
    [Theory]
    [InlineData(8, 3)]
    [InlineData(7, 4)]
    [InlineData(4, 2)]
    [InlineData(1, 0)]
    [InlineData(2147483647, 32)]
    public void IntegerReplacement_LeetCodeExamples_ReturnsMinimumStepCount(long n, int expected)
        => Assert.Equal(expected, IntegerReplacementSteps(n));

    private static int IntegerReplacementSteps(long n)
        => Memoizer.Memoize<long, int>(n, (value, replace) => value switch
        {
            1 => 0,
            _ when value % 2 == 0 => 1 + replace(value / 2),
            _ => 1 + Math.Min(replace(value - 1), replace(value + 1)),
        });
}
