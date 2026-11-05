using DSAExperimentation.LeetCode.Solutions.NumberOfCommonFactors;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfCommonFactors;

// Harness only: both algorithms live in NumberOfCommonFactorsSolution. One test
// method per strategy over one shared set of cases, so a failure names the
// strategy that broke rather than reporting a disagreement between two anonymous
// arms. The cases include a gcd that is a perfect square, whose root the divisor
// enumeration must count once. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class NumberOfCommonFactorsSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            // gcd = 5, whose divisors are 1 and 5.
            { 25, 15, 2 },

            // gcd = 12, whose divisors are 1, 2, 3, 4, 6, 12.
            { 36, 24, 6 },

            // gcd = 100 is a perfect square, so i*i == gcd offers 10 twice and the
            // set has to collapse it: 1, 2, 4, 5, 10, 20, 25, 50, 100.
            { 100, 100, 9 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountCommonFactorsByLinearScan_Cases_ReturnsCommonFactorCount(
        int first, int second, int expected)
    {
        var actual = NumberOfCommonFactorsSolution.CountCommonFactorsByLinearScan(first, second);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountCommonFactorsByDivisorEnumeration_Cases_ReturnsCommonFactorCount(
        int first, int second, int expected)
    {
        var actual = NumberOfCommonFactorsSolution.CountCommonFactorsByDivisorEnumeration(first, second);

        Assert.Equal(expected, actual);
    }
}
