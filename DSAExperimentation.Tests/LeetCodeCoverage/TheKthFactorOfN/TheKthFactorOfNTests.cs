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
    {
        var factor = KthFactor(n, k);
        Assert.Equal(expected, factor);
    }

    private static int KthFactor(int n, int k)
    {
        var anchor = FindAnchor(n);
        var remaining = k;

        if (TryFindAscending(n, anchor, ref remaining, out var found))
        {
            return found;
        }

        if (TryFindDescending(n, anchor, ref remaining, out found))
        {
            return found;
        }

        return -1;
    }

    private static int FindAnchor(int n)
    {
        var sequence = new SquareExceedsSequence(n, n + 1);
        return BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;
    }

    private static bool TryFindAscending(int n, int anchor, ref int remaining, out int found)
    {
        for (var divisor = 1; divisor <= anchor; divisor++)
        {
            if (n % divisor == 0 && --remaining == 0)
            {
                found = divisor;
                return true;
            }
        }

        found = -1;
        return false;
    }

    private static bool TryFindDescending(int n, int anchor, ref int remaining, out int found)
    {
        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            if (divisor * divisor == n || n % divisor != 0)
            {
                continue;
            }

            if (--remaining == 0)
            {
                found = n / divisor;
                return true;
            }
        }

        found = -1;
        return false;
    }

    private readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;
        public int Get(int value) => (long)value * value > x ? 1 : 0;
    }
}
