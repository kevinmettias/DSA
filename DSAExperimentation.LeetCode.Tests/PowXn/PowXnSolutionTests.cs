using DSAExperimentation.LeetCode.PowXn;

namespace DSAExperimentation.LeetCode.Tests.PowXn;

// Harness only. Both strategies are PowXnSolution's; this file just pins them to
// LeetCode's published examples, plus the int.MinValue exponent that would
// overflow a plain int negation and forces both strategies through the long-typed
// exponent path instead. LeetCode prints its answers to five decimal places, so
// 2.1^3 = 9.261 is compared at the same six-place precision as every other row.
public sealed partial class PowXnSolutionTests
{
    public static TheoryData<double, int, double> Examples =>
        new()
        {
            // LeetCode examples 1-3.
            { 2.0, 10, 1024.0 },
            { 2.1, 3, 9.261 },
            { 2.0, -2, 0.25 },

            // 1 to any power is 1, at the exponent a plain int negation overflows.
            { 1.0, int.MinValue, 1.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PowByRepeatedMultiplication_LeetCodeExamples_ReturnsExpected(
        double baseValue, int power, double expected)
    {
        var actual = PowXnSolution.PowByRepeatedMultiplication(baseValue, power);

        Assert.Equal(expected, actual, 6);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PowByExponentiationBySquaring_LeetCodeExamples_ReturnsExpected(
        double baseValue, int power, double expected)
    {
        var actual = PowXnSolution.PowByExponentiationBySquaring(baseValue, power);

        Assert.Equal(expected, actual, 6);
    }
}
