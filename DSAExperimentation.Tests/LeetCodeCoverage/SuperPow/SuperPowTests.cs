namespace DSAExperimentation.Tests.LeetCodeCoverage.SuperPow;

// LeetCode 372. Super Pow: a^b mod 1337 where b is an arbitrarily long exponent given
// digit-by-digit (up to 2000 digits - far past any fixed-width integer). Horner's rule
// over the digits combined with modular exponentiation by squaring keeps every
// intermediate value inside [0, 1337) and does O(digits) work total. No repo container
// or algorithm primitive applies here - there is nothing to compose over a handful of
// running scalars (result/base/exponent), the same "lighter repo-primitive fit" case
// this repo already accepted for Pow(x, n) and Power of Two.
public sealed class SuperPowTests
{
    private const int Modulus = 1337;

    [Theory]
    [InlineData(2, new[] { 3 }, 8)]
    [InlineData(2, new[] { 1, 0 }, 1024)]
    [InlineData(1, new[] { 4, 3, 3, 8, 5, 2 }, 1)]
    [InlineData(3, new[] { 1, 0 }, 221)] // 3^10 = 59049, 59049 mod 1337 = 221
    public void SuperPow_LeetCodeExamples_ReturnsModularPower(int a, int[] b, int expected)
        => Assert.Equal(expected, SuperPow(a, b));

    private static int SuperPow(int a, int[] b)
    {
        var result = 1L;
        var baseTerm = a % Modulus;

        foreach (var digit in b)
        {
            result = ModPow(result, 10) * ModPow(baseTerm, digit) % Modulus;
        }

        return (int)result;
    }

    // Exponentiation by squaring, exactly PowXn's loop, folded under Modulus at every
    // multiplication so intermediate values never grow past Modulus^2.
    private static long ModPow(long value, int exponent)
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
