using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountingBits;

// LeetCode 338. Counting Bits: the same Memoizer-driven DP composition
// HouseRobberTests/HouseRobberIITests already use, applied to the classic
// ans[i] = ans[i>>1] + (i&1) recurrence instead of recounting each value's set bits
// from scratch.
public sealed partial class CountingBitsTests
{
    [Theory]
    [InlineData(2, new[] { 0, 1, 1 })]
    [InlineData(5, new[] { 0, 1, 1, 2, 1, 2 })]
    public void CountBits_Examples_ReturnsBitCountsForEveryIndex(int n, int[] expected)
        => Assert.Equal(expected, CountBits(n));

    private static int[] CountBits(int n)
    {
        var result = new int[n + 1];

        for (var i = 0; i <= n; i++)
        {
            result[i] = Memoizer.Memoize<int, int>(
                i, (value, countBits) => value == 0 ? 0 : countBits(value >> 1) + (value & 1));
        }

        return result;
    }
}
