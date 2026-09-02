namespace DSAExperimentation.Tests.LeetCodeCoverage.CountGoodNumbers;

// LeetCode 1922. Count Good Numbers: a digit string of length n is "good" when every
// even index holds one of 5 even digits and every odd index holds one of 4 prime
// digits, so the count is 5^ceil(n/2) * 4^floor(n/2) mod 1e9+7 - n can be up to 1e15,
// so this has to be modular exponentiation by squaring, not repeated multiplication.
// No repo container or algorithm primitive applies here - there is nothing to
// compose over two running scalars (result/base), the same "lighter repo-primitive
// fit" this repo already accepted for Pow(x, n) and Super Pow.
public sealed class CountGoodNumbersTests
{
    private const long Modulo = 1_000_000_007;

    [Theory]
    [InlineData(1, 5)]
    [InlineData(4, 400)]
    [InlineData(50, 564908303)]
    public void CountGoodNumbers_LeetCodeExamples_ReturnsExpectedCount(long n, int expected)
        => Assert.Equal(expected, CountGoodNumbers(n));

    private static int CountGoodNumbers(long n)
    {
        var evenPositions = (n + 1) / 2;
        var oddPositions = n / 2;

        return (int)(ModPow(5, evenPositions) * ModPow(4, oddPositions) % Modulo);
    }

    // Exactly PowXn's exponentiation-by-squaring loop, folded under Modulo at every
    // multiplication (SuperPow's own adaptation of the same loop) so intermediate
    // values never grow past Modulo^2.
    private static long ModPow(long value, long exponent)
    {
        var result = 1L;
        value %= Modulo;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % Modulo;
            }

            value = value * value % Modulo;
            exponent >>= 1;
        }

        return result;
    }
}
