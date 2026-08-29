using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DivideTwoIntegers;

public sealed partial class DivideTwoIntegersTests
{
    [Theory]
    [InlineData(10, 3, 3)]
    [InlineData(7, -3, -2)]
    [InlineData(int.MinValue, -1, int.MaxValue)]
    [InlineData(int.MinValue, 1, int.MinValue)]
    public void Divide_LeetCodeExamples_TruncatesTowardZero(int dividend, int divisor, int expected)
        => Assert.Equal(expected, Divide(dividend, divisor));

    private static int Divide(int dividend, int divisor)
    {
        if (dividend == int.MinValue && divisor == -1) return int.MaxValue;
        if (dividend == int.MinValue && divisor == 1) return int.MinValue;

        var negative = (dividend < 0) ^ (divisor < 0);
        var absDividend = Math.Abs((long)dividend);
        var absDivisor = Math.Abs((long)divisor);
        var maxCandidate = (int)Math.Min(int.MaxValue, absDividend);
        var sequence = new ProductExceedsSequence(absDivisor, absDividend, maxCandidate + 1);
        var firstTooLarge = BinarySearch.LowerBound<int, ProductExceedsSequence>(sequence, 1);
        var quotient = firstTooLarge - 1;

        return negative ? -quotient : quotient;
    }

    private readonly struct ProductExceedsSequence(long divisor, long dividend, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int quotient) => divisor * quotient > dividend ? 1 : 0;
    }
}

