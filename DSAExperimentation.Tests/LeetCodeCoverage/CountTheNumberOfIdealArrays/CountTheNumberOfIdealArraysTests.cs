using DSAExperimentation.LeetCode.CountTheNumberOfIdealArrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheNumberOfIdealArrays;

// Harness only. Both factorization strategies are
// CountTheNumberOfIdealArraysSolution's; this file pins them to LeetCode's published
// examples plus the degenerate length-one case and a maxValue whose values include a
// prime power (8 = 2^3), which is the one shape that exercises an exponent above one
// dividing out completely inside the trial-division loop with no leftover prime.
// IdealArraysByTrialDivision was previously the benchmark's untested baseline arm.
public sealed partial class CountTheNumberOfIdealArraysTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 2, 5, 10 },
            { 5, 3, 11 },
            { 1, 10, 10 },
            { 3, 1, 1 },
            { 4, 8, 63 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IdealArraysByTrialDivision_LeetCodeExamples_ReturnsExpectedCount(
        int arrayLength, int maxValue, int expected)
    {
        var actual = CountTheNumberOfIdealArraysSolution.IdealArraysByTrialDivision(arrayLength, maxValue);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IdealArraysBySmallestPrimeFactorSieve_LeetCodeExamples_ReturnsExpectedCount(
        int arrayLength, int maxValue, int expected)
    {
        var actual = CountTheNumberOfIdealArraysSolution.IdealArraysBySmallestPrimeFactorSieve(
            arrayLength, maxValue);
        Assert.Equal(expected, actual);
    }
}
