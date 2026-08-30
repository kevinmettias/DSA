namespace DSAExperimentation.Tests.LeetCodeCoverage.PowXn;

// LeetCode 50. Pow(x, n): iterative exponentiation by squaring over a single running
// scalar. No repo container or algorithm primitive applies here - there is nothing
// to compose over one double and one exponent counter, the same "lighter
// repo-primitive fit" case as Power of Two's bit trick.
public sealed class PowXnTests
{
    [Theory]
    [InlineData(2.0, 10, 1024.0)]
    [InlineData(2.0, -2, 0.25)]
    [InlineData(1.0, int.MinValue, 1.0)]
    public void MyPow_ExponentiationBySquaring_ReturnsExpected(double x, int n, double expected)
        => Assert.Equal(expected, MyPow(x, n), 6);

    private static double MyPow(double x, int n)
    {
        long exponent = n;

        if (exponent < 0)
        {
            x = 1 / x;
            exponent = -exponent;
        }

        var result = 1.0;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result *= x;
            }

            x *= x;
            exponent >>= 1;
        }

        return result;
    }
}
