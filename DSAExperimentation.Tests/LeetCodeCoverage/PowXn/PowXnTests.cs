using DSAExperimentation.LeetCode.PowXn;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PowXn;

// Harness only. Both strategies are PowXnSolution's; this file just pins them to
// LeetCode's published examples, including the int.MinValue exponent that would
// overflow a plain int negation and forces both strategies through the long-typed
// exponent path instead.
public sealed class PowXnTests
{
    public static TheoryData<double, int, double> Examples =>
        new()
        {
            { 2.0, 10, 1024.0 },
            { 2.0, -2, 0.25 },
            { 1.0, int.MinValue, 1.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PowByRepeatedMultiplication_LeetCodeExamples_ReturnsExpected(double x, int n, double expected)
    {
        var actual = PowXnSolution.PowByRepeatedMultiplication(x, n);

        Assert.Equal(expected, actual, 6);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PowByExponentiationBySquaring_LeetCodeExamples_ReturnsExpected(double x, int n, double expected)
    {
        var actual = PowXnSolution.PowByExponentiationBySquaring(x, n);

        Assert.Equal(expected, actual, 6);
    }
}
