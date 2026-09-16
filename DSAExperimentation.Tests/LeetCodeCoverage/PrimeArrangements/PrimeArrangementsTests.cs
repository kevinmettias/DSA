using DSAExperimentation.LeetCode.PrimeArrangements;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrimeArrangements;

// Harness only. Both strategies are PrimeArrangementsSolution's - this file pins
// them to LeetCode's published examples plus the small-upperBound boundaries
// where the prime count is zero or one and the factorials collapse to 1.
public sealed class PrimeArrangementsTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 5, 12 }, // LC's example 1: 3 primes, 2 others - 3! * 2!
            { 100, 682289015 }, // LC's example 2
            { 1, 1 }, // no primes at all
            { 2, 1 }, // 1! * 1!
            { 3, 2 }, // 2! * 1!
            { 10, 17280 }, // 4! * 6!
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPrimeArrangementsByTrialDivision_LeetCodeExamples_ReturnsArrangementCount(
        int upperBound, int expected) =>
        Assert.Equal(expected, PrimeArrangementsSolution.CountPrimeArrangementsByTrialDivision(upperBound));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPrimeArrangementsBySieveOfEratosthenes_LeetCodeExamples_ReturnsArrangementCount(
        int upperBound, int expected) =>
        Assert.Equal(expected, PrimeArrangementsSolution.CountPrimeArrangementsBySieveOfEratosthenes(upperBound));
}
