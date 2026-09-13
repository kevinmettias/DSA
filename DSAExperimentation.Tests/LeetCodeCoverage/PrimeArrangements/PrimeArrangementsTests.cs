using DSAExperimentation.LeetCode.PrimeArrangements;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrimeArrangements;

// Harness only. Both strategies are PrimeArrangementsSolution's - this file pins
// them to LeetCode's published examples plus the small-n boundaries where the
// prime count is zero or one and the factorials collapse to 1.
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
    public void NumPrimeArrangementsByTrialDivision_LeetCodeExamples_ReturnsArrangementCount(
        int n, int expected) =>
        Assert.Equal(expected, PrimeArrangementsSolution.NumPrimeArrangementsByTrialDivision(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumPrimeArrangementsBySieveOfEratosthenes_LeetCodeExamples_ReturnsArrangementCount(
        int n, int expected) =>
        Assert.Equal(expected, PrimeArrangementsSolution.NumPrimeArrangementsBySieveOfEratosthenes(n));
}
