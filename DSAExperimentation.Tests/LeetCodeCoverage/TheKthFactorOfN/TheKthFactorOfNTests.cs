using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheKthFactorOfN;

// LeetCode 1492. The kth Factor of n: the same anchor-at-floor(sqrt(n)) technique
// FourDivisorsTests/ClosestDivisorsTests use for their own divisor enumeration -
// BinarySearch.LowerBound over a monotone virtual SquareExceedsSequence locates
// floor(sqrt(n)) in O(log n) - then walks divisors up to the anchor (ascending) and,
// past it, walks the anchor back down to 1 pairing each with n/divisor (also
// ascending, since n/divisor grows as divisor shrinks), counting off factors in
// sorted order until the kth is found.
public sealed partial class TheKthFactorOfNTests
{
    [Theory]
    [InlineData(12, 3, 3)]
    [InlineData(7, 2, 7)]
    [InlineData(4, 4, -1)]
    [InlineData(1, 1, 1)]
    public void KthFactor_LeetCodeExamples_ReturnsExpectedFactorOrMinusOne(int n, int k, int expected)
        => Assert.Equal(expected, KthFactor(n, k));

    private static int KthFactor(int n, int k)
    {
        var sequence = new SquareExceedsSequence(n, n + 1);
        var anchor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;
        var remaining = k;

        for (var divisor = 1; divisor <= anchor; divisor++)
        {
            if (n % divisor == 0 && --remaining == 0)
            {
                return divisor;
            }
        }

        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            if (divisor * divisor == n || n % divisor != 0)
            {
                continue;
            }

            if (--remaining == 0)
            {
                return n / divisor;
            }
        }

        return -1;
    }

    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int value) => (long)value * value > x ? 1 : 0;
    }
}
