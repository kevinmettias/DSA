namespace DSAExperimentation.Tests.LeetCodeCoverage.CountCollisionsOfMonkeysOnAPolygon;

// LeetCode 2550. Count Collisions of Monkeys on a Polygon: each of the n
// monkeys independently picks one of 2 directions, so there are 2^n total
// configurations; the only two configurations with zero collisions are "every
// monkey goes clockwise" and "every monkey goes counterclockwise" (any other
// mix has two adjacent monkeys meeting somewhere on the polygon), so the
// answer is (2^n - 2) mod 1e9+7. Fast exponentiation by squaring is the same
// private-helper-for-a-small-numeric-primitive pattern this repo already
// reuses when no DataStructures/Algorithms type applies - the precedent
// FindGreatestCommonDivisorOfArrayTests sets for its own Euclidean Gcd
// ("no repo container or algorithm primitive applies here").
public sealed partial class CountCollisionsOfMonkeysOnAPolygonTests
{
    private const int Mod = 1_000_000_007;

    [Theory]
    [InlineData(3, 6)]
    [InlineData(4, 14)]
    public void NumberOfWays_LeetCodeExamples_ReturnsCollisionCount(int n, int expected)
    {
        var actual = NumberOfWays(n);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NumberOfWays_LargeN_StaysWithinModulusBounds()
    {
        var actual = NumberOfWays(100_000);
        Assert.InRange(actual, 0, Mod - 1);
    }

    private static int NumberOfWays(int n) => (int)((ModPow(2, n, Mod) - 2 + Mod) % Mod);

    private static long ModPow(long value, long exponent, long modulus)
    {
        var result = 1L;
        value %= modulus;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % modulus;
            }

            value = value * value % modulus;
            exponent >>= 1;
        }

        return result;
    }
}
