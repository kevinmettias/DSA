namespace DSAExperimentation.Tests.LeetCodeCoverage.PowXn;

public sealed class PowXnTests
{
    [Theory]
    [InlineData(2.0, 10, 1024.0)]
    [InlineData(2.0, -2, 0.25)]
    [InlineData(1.0, int.MinValue, 1.0)]
    public void MyPow_ExponentiationBySquaring_ReturnsExpected(double x, int n, double expected) => Assert.Equal(expected, MyPow(x, n), 6);

    private static double MyPow(double x, int n)
    {
        var exp = (long)n; if (exp < 0) { x = 1 / x; exp = -exp; }
        var result = 1.0;
        while (exp > 0) { if ((exp & 1) == 1) result *= x; x *= x; exp >>= 1; }
        return result;
    }
}
