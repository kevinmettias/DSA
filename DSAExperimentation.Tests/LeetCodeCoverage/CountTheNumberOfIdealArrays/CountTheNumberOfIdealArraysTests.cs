using DSAExperimentation.LeetCode.CountTheNumberOfIdealArrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheNumberOfIdealArrays;

// Harness only. Both factorization strategies are
// CountTheNumberOfIdealArraysSolution's; this file pins them to LeetCode's published
// examples plus the degenerate length-one case and a maxValue whose values include a
// prime power (8 = 2^3), which is the one shape that exercises an exponent above one
// dividing out completely inside the trial-division loop with no leftover prime.
// IdealArraysByTrialDivision was previously the benchmark's untested baseline arm.
public sealed class CountTheNumberOfIdealArraysTests
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
        int n, int maxValue, int expected) =>
        Assert.Equal(expected, CountTheNumberOfIdealArraysSolution.IdealArraysByTrialDivision(n, maxValue));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IdealArraysBySmallestPrimeFactorSieve_LeetCodeExamples_ReturnsExpectedCount(
        int n, int maxValue, int expected) =>
        Assert.Equal(
            expected,
            CountTheNumberOfIdealArraysSolution.IdealArraysBySmallestPrimeFactorSieve(n, maxValue));
}
