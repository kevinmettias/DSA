using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UglyNumberIII;

// LeetCode 1201. Ugly Number III: count(x) = x/a + x/b + x/c - x/lcm(a,b) -
// x/lcm(a,c) - x/lcm(b,c) + x/lcm(a,b,c) (three-set inclusion-exclusion over
// multiples of a, b, c) is non-decreasing in x, so the nth ugly number is
// BinarySearch.LowerBound's first index whose count reaches n - the same
// "monotone virtual sequence" shape NthMagicalNumberTests already uses for two
// factors, extended to three. lcm(a, b, c) is computed via a capped Lcm so that
// combining two already-huge pairwise LCMs never overflows long: once one operand
// already exceeds the search's upperBound, every further multiple of it does too,
// so the exact value stops mattering and it can be clamped instead of multiplied.
public sealed partial class UglyNumberIIITests
{
    [Fact]
    public void NthUglyNumber_LeetCodeExampleOne_ReturnsFourthCombinedMultiple()
    {
        var result = NthUglyNumber(n: 3, a: 2, b: 3, c: 5);

        Assert.Equal(4, result);
    }

    [Fact]
    public void NthUglyNumber_LeetCodeExampleTwo_ReturnsSixthCombinedMultiple()
    {
        var result = NthUglyNumber(n: 4, a: 2, b: 3, c: 4);

        Assert.Equal(6, result);
    }

    [Fact]
    public void NthUglyNumber_FirstRequestedNumber_ReturnsSmallestFactor()
    {
        var result = NthUglyNumber(n: 1, a: 4, b: 6, c: 9);

        Assert.Equal(4, result);
    }

    private static int NthUglyNumber(int n, int a, int b, int c)
    {
        var lcmAb = Lcm(a, b);
        var lcmAc = Lcm(a, c);
        var lcmBc = Lcm(b, c);
        var minBc = Math.Min(b, c);
        var upperBound = checked((int)((long)n * Math.Min(a, minBc)));
        var lcmAbc = LcmCapped(lcmAb, c, upperBound);

        var sequence = new UglyCountSequence(n, a, b, c, lcmAb, lcmAc, lcmBc, lcmAbc, upperBound);

        return BinarySearch.LowerBound<int, UglyCountSequence>(sequence, 1);
    }

    private static long Gcd(long x, long y) => y == 0 ? x : Gcd(y, x % y);

    private static long Lcm(long x, long y) => x / Gcd(x, y) * y;

    // x is only ever multiplied once it is already within cap, so the product
    // never exceeds cap * y - safely inside long's range for this problem's
    // 10^9-bounded factors.
    private static long LcmCapped(long x, long y, long cap) => x > cap ? cap + 1 : Lcm(x, y);

    // Get(index) treats index itself as the candidate ugly number x, the same
    // "value doubles as index" shape MagicalCountSequence already uses.
    private readonly struct UglyCountSequence(
        int n, int a, int b, int c, long lcmAb, long lcmAc, long lcmBc, long lcmAbc, int upperBound)
        : IRandomAccessSequence<int>
    {
        public int Length => upperBound + 1;

        public int Get(int index)
        {
            long x = index;
            var count = (x / a) + (x / b) + (x / c) - (x / lcmAb) - (x / lcmAc) - (x / lcmBc) + (x / lcmAbc);
            return count >= n ? 1 : 0;
        }
    }
}
