using DSAExperimentation.LeetCode.MaximizeNumberOfNiceDivisors;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeNumberOfNiceDivisors;

// Harness only. Both recurrences are MaximizeNumberOfNiceDivisorsSolution's; this
// file pins them to LeetCode's two published examples plus the small budgets where
// leaving the budget whole still beats splitting it, and the first budgets at which
// peeling a 3 overtakes peeling a 2.
public sealed class MaximizeNumberOfNiceDivisorsTests
{
    // 57 = 3 * 19, so the exact product is 3^19 = 1,162,261,467 - the smallest
    // multiple-of-three budget whose answer overflows LeetCode's own 1e9+7 modulus
    // and therefore the smallest one that proves the reported answer is reduced.
    private const int OverflowingPrimeFactors = 57;
    private const int OverflowingExpected = 162_261_460;

    public static TheoryData<int, int> Examples =>
        new()
        {
            { 1, 1 },
            { 2, 2 },
            { 3, 3 },
            { 4, 4 },
            { 5, 6 },
            { 6, 9 },
            { 7, 12 },
            { 8, 18 },
            { 9, 27 },
            { 10, 36 },
            { 20, 1_458 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxNiceDivisorsByNaiveRecursion_LeetCodeExamples_ReturnsMaximumNiceDivisorCount(
        int primeFactors, int expected) =>
        Assert.Equal(expected, MaximizeNumberOfNiceDivisorsSolution.MaxNiceDivisorsByNaiveRecursion(primeFactors));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxNiceDivisorsByMemoizedRecurrence_LeetCodeExamples_ReturnsMaximumNiceDivisorCount(
        int primeFactors, int expected) =>
        Assert.Equal(expected, MaximizeNumberOfNiceDivisorsSolution.MaxNiceDivisorsByMemoizedRecurrence(primeFactors));

    // The naive arm deliberately has no cache, so it cannot be run at a budget this
    // large in a unit test - the same asymmetry the benchmark's [Params] bound
    // records. Only the memoized arm is asserted here.
    [Fact]
    public void MaxNiceDivisorsByMemoizedRecurrence_ProductExceedsLeetCodesPrime_ReportsItModuloThatPrime() =>
        Assert.Equal(
            OverflowingExpected,
            MaximizeNumberOfNiceDivisorsSolution.MaxNiceDivisorsByMemoizedRecurrence(OverflowingPrimeFactors));
}
