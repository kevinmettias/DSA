using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NthMagicalNumber;

// LeetCode 878. Nth Magical Number: count(x) = x/a + x/b - x/lcm(a,b) (inclusion-
// exclusion over multiples of a and b) is non-decreasing in x, so the nth magical
// number is BinarySearch.LowerBound's first index whose count reaches n - the same
// "monotone virtual sequence" shape SqrtXTests/FirstBadVersionTests already use,
// just with a two-term counting predicate per index instead of a single comparison.
public sealed partial class NthMagicalNumberTests
{
    private const int Modulus = 1_000_000_007;

    [Fact]
    public void NthMagicalNumber_LeetCodeExampleOne_ReturnsFirstMultiple()
    {
        var result = NthMagicalNumber(n: 1, a: 2, b: 3);

        Assert.Equal(2, result);
    }

    [Fact]
    public void NthMagicalNumber_SharedFactorBetweenAAndB_CountsSharedMultiplesOnce()
    {
        // b is a multiple of a, so magical numbers are exactly the multiples of a
        // (2, 4, 6, 8, ...) - a inclusion-exclusion subtracts the full overlap.
        var result = NthMagicalNumber(n: 4, a: 2, b: 4);

        Assert.Equal(8, result);
    }

    [Fact]
    public void NthMagicalNumber_CoprimeFactors_InterleavesBothSequences()
    {
        // Multiples of 3 or 5 in order: 3, 5, 6, 9, 10, 12, 15, ... the 7th is 15.
        var result = NthMagicalNumber(n: 7, a: 3, b: 5);

        Assert.Equal(15, result);
    }

    private static int NthMagicalNumber(int n, int a, int b)
    {
        var lcm = (long)a / Gcd(a, b) * b;
        var upperBound = checked((int)((long)n * Math.Min(a, b)));
        var sequence = new MagicalCountSequence(n, a, b, lcm, upperBound);

        var x = BinarySearch.LowerBound<int, MagicalCountSequence>(sequence, 1);

        return (int)((long)x % Modulus);
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    // Get(index) treats index itself as the candidate magical number x, exactly the
    // "value doubles as index" shape SqrtXTests' SquareExceedsSequence already uses -
    // 1 once count(x) reaches n, 0 while it hasn't.
    private readonly struct MagicalCountSequence(int n, int a, int b, long lcm, int upperBound)
        : IRandomAccessSequence<int>
    {
        public int Length => upperBound + 1;

        public int Get(int index) => (index / a) + (index / b) - (index / lcm) >= n ? 1 : 0;
    }
}
