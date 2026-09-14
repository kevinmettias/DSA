using DSAExperimentation.LeetCode.NumberOfCommonFactors;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfCommonFactors;

// Harness only: both algorithms live in NumberOfCommonFactorsSolution. One test
// method per strategy over one shared set of examples, so a failure names the
// strategy that broke rather than reporting a disagreement between two anonymous
// arms.
public sealed class NumberOfCommonFactorsTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            // LeetCode's own two examples.
            { 12, 6, 4 },
            { 25, 15, 2 },

            // The smallest possible input.
            { 1, 1, 1 },

            // Coprime operands: only 1 divides both.
            { 7, 13, 1 },

            // gcd = 12, whose divisors are 1, 2, 3, 4, 6, 12.
            { 36, 24, 6 },

            // gcd = 100 is a perfect square, so i*i == gcd offers 10 twice and the
            // set has to collapse it: 1, 2, 4, 5, 10, 20, 25, 50, 100.
            { 100, 100, 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCommonFactorsByLinearScan_LeetCodeExamples_ReturnsCommonFactorCount(
        int first, int second, int expected) =>
        Assert.Equal(expected, NumberOfCommonFactorsSolution.CountCommonFactorsByLinearScan(first, second));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCommonFactorsByDivisorEnumeration_LeetCodeExamples_ReturnsCommonFactorCount(
        int first, int second, int expected) =>
        Assert.Equal(expected, NumberOfCommonFactorsSolution.CountCommonFactorsByDivisorEnumeration(first, second));
}
