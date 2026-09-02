using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ThreeDivisors;

// LeetCode 1952. Three Divisors: a number has exactly three divisors iff it is the
// square of a prime (1, p, p^2), so this is the same anchor-at-floor(sqrt) technique
// FourDivisorsTests/ClosestDivisorsTests use for LC 1390/1362 - BinarySearch.LowerBound
// over a monotone virtual SquareExceedsSequence locates floor(sqrt(num)) in O(log num),
// then a downward walk collects divisor pairs, bailing out the moment a fourth divisor
// appears (so any number with more than three divisors never pays for a full scan to 1).
public sealed partial class ThreeDivisorsTests
{
    [Theory]
    [InlineData(2, false)] // divisors: 1, 2
    [InlineData(4, true)] // divisors: 1, 2, 4
    [InlineData(9, true)] // divisors: 1, 3, 9 (3^2)
    [InlineData(100, false)] // 2^2 * 5^2 has nine divisors
    public void IsThree_LeetCodeAndSquarePrimeExamples_MatchesDivisorCount(int num, bool expected)
        => Assert.Equal(expected, HasExactlyThreeDivisors(num));

    private static bool HasExactlyThreeDivisors(int num)
    {
        var sequence = new SquareExceedsSequence(num, Math.Min(num, 46_341) + 1);
        var anchor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;

        var count = 0;

        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            if (num % divisor != 0)
            {
                continue;
            }

            var paired = num / divisor;
            count += divisor == paired ? 1 : 2;

            if (count > 3)
            {
                return false;
            }
        }

        return count == 3;
    }

    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int value) => (long)value * value > x ? 1 : 0;
    }
}
