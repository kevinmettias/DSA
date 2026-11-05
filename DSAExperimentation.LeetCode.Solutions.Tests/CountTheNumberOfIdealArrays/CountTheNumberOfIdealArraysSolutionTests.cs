using DSAExperimentation.LeetCode.Solutions.CountTheNumberOfIdealArrays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountTheNumberOfIdealArrays;

// Harness only. Both factorization strategies are
// CountTheNumberOfIdealArraysSolution's; this file pins them to the degenerate
// length-one case and a maxValue whose values include a prime power (8 = 2^3), which
// is the one shape that exercises an exponent above one dividing out completely
// inside the trial-division loop with no leftover prime.
// IdealArraysByTrialDivision was previously the benchmark's untested baseline arm.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CountTheNumberOfIdealArraysSolutionTests
{
    public static TheoryData<int, int, int> Cases =>
        new()
        {
            { 1, 10, 10 },
            { 4, 8, 63 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IdealArraysByTrialDivision_Cases_ReturnsExpectedCount(
        int arrayLength, int maxValue, int expected)
    {
        var actual = CountTheNumberOfIdealArraysSolution.IdealArraysByTrialDivision(arrayLength, maxValue);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IdealArraysBySmallestPrimeFactorSieve_Cases_ReturnsExpectedCount(
        int arrayLength, int maxValue, int expected)
    {
        var actual = CountTheNumberOfIdealArraysSolution.IdealArraysBySmallestPrimeFactorSieve(
            arrayLength, maxValue);
        Assert.Equal(expected, actual);
    }
}
