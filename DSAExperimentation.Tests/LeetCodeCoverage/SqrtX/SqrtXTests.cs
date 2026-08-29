using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SqrtX;

// LeetCode 69. Sqrt(x): BinarySearch.LowerBound finds the first integer whose
// square exceeds x over a monotone virtual sequence, then steps back.
public sealed partial class SqrtXTests
{
    [Theory]
    [InlineData(4, 2)]
    [InlineData(8, 2)]
    [InlineData(2147395599, 46339)]
    public void MySqrt_LeetCodeExamples_ReturnsFloorRoot(int x, int expected)
        => Assert.Equal(expected, MySqrt(x));

    private static int MySqrt(int x)
    {
        var sequence = new SquareExceedsSequence(x, Math.Min(x, 46341) + 1);
        return BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;
    }

    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int value) => (long)value * value > x ? 1 : 0;
    }
}
