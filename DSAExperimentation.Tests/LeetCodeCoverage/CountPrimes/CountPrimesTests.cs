using DSAExperimentation.LeetCode.CountPrimes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountPrimes;

// Harness only: both strategies live in CountPrimesSolution. One test method
// per strategy over one shared set of LeetCode's own examples, so a failure
// names the strategy that broke.
public sealed class CountPrimesTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 10, 4 },
            { 0, 0 },
            { 1, 0 },
            { 2, 0 },
            { 3, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPrimesByTrialDivision_LeetCodeExamples_ReturnsPrimeCount(int limit, int expected) =>
        Assert.Equal(expected, CountPrimesSolution.CountPrimesByTrialDivision(limit));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPrimesBySieveOfEratosthenes_LeetCodeExamples_ReturnsPrimeCount(int limit, int expected) =>
        Assert.Equal(expected, CountPrimesSolution.CountPrimesBySieveOfEratosthenes(limit));
}
