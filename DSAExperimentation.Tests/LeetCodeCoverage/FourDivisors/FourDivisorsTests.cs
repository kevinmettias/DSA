using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FourDivisors;

// LeetCode 1390. Four Divisors: the same anchor-at-floor(sqrt) technique
// ClosestDivisorsTests uses - BinarySearch.LowerBound over a monotone virtual
// SquareExceedsSequence locates floor(sqrt(num)) in O(log num), then a downward
// walk collects every divisor pair from there, summing them and bailing out the
// moment a fifth divisor appears (so a number with more than four divisors never
// pays for a full scan down to 1).
public sealed partial class FourDivisorsTests
{
    [Theory]
    [InlineData(new[] { 21, 4, 7 }, 32)]
    [InlineData(new[] { 21, 21 }, 64)]
    public void SumFourDivisors_LeetCodeExamples_ReturnsSumOverQualifyingNumbers(int[] nums, int expected)
        => Assert.Equal(expected, SumFourDivisors(nums));

    private static int SumFourDivisors(int[] nums)
    {
        var total = 0;

        foreach (var num in nums)
        {
            total += DivisorSumIfExactlyFour(num);
        }

        return total;
    }

    private static int DivisorSumIfExactlyFour(int num)
    {
        var sequence = new SquareExceedsSequence(num, Math.Min(num, 46_341) + 1);
        var anchor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;

        var count = 0;
        var sum = 0;

        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            if (num % divisor != 0)
            {
                continue;
            }

            var paired = num / divisor;
            count += divisor == paired ? 1 : 2;
            sum += divisor == paired ? divisor : divisor + paired;

            if (count > 4)
            {
                return 0;
            }
        }

        return count == 4 ? sum : 0;
    }

    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int value) => (long)value * value > x ? 1 : 0;
    }
}
