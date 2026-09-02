namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNonZeroProductOfTheArrayElements;

// LeetCode 1969. Minimum Non-Zero Product of the Array Elements: pairing i with
// (2^p-1-i) for every i in [1, 2^p-2] and bit-swapping each pair toward its
// extremes (1 and 2^p-2) minimizes that pair's product while its sum stays fixed;
// the untouched top value 2^p-1 is left alone. The product collapses to
// (2^p-2)^(2^(p-1)-1) * (2^p-1) mod 1e9+7, computed via exponentiation by
// squaring for the one genuinely large exponent - the same "lighter
// repo-primitive fit" this repo already accepted for Pow(x, n), Super Pow, and
// Power of Two: nothing here to compose over a handful of running scalars.
public sealed class MinimumNonZeroProductOfTheArrayElementsTests
{
    private const long Modulus = 1_000_000_007;

    [Theory]
    [InlineData(1, 1L)]
    [InlineData(2, 6L)]
    [InlineData(3, 1512L)]
    public void MinNonZeroProduct_LeetCodeExamples_ReturnsMinimalProduct(int p, long expected)
        => Assert.Equal(expected, MinNonZeroProduct(p));

    private static long MinNonZeroProduct(int p)
    {
        var largestValue = (1L << p) - 1; // 2^p - 1, exact for p <= 60 (fits in long)
        var pairBase = largestValue - 1; // 2^p - 2, every pair's product once bit-swapped apart
        var pairCount = (1L << (p - 1)) - 1; // 2^(p-1) - 1 pairs summing to 2^p - 1 each

        return ModPow(pairBase, pairCount) * (largestValue % Modulus) % Modulus;
    }

    // Exponentiation by squaring, exactly PowXn's/SuperPow's loop, folded under Modulus at
    // every multiplication so intermediate values never grow past Modulus^2.
    private static long ModPow(long value, long exponent)
    {
        var result = 1L;
        value %= Modulus;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % Modulus;
            }

            value = value * value % Modulus;
            exponent >>= 1;
        }

        return result;
    }
}
