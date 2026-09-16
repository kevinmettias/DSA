using DSAExperimentation.LeetCode.CheckIfItIsAGoodArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfItIsAGoodArray;

// Harness only. Both gcd strategies are CheckIfItIsAGoodArraySolution's - this
// file just pins them to LeetCode's published examples plus the degenerate
// single-element cases and a set whose values are pairwise non-coprime yet whose
// overall gcd is still 1.
public sealed class CheckIfItIsAGoodArrayTests
{
    public static TheoryData<GoodArrayCase> Examples =>
        new()
        {
            { new GoodArrayCase([12, 5, 7, 23], Expected: true) },
            { new GoodArrayCase([29, 6, 10], Expected: true) },
            { new GoodArrayCase([3, 6], Expected: false) },
            { new GoodArrayCase([6, 10, 15], Expected: true) },
            { new GoodArrayCase([2, 4, 8], Expected: false) },
            { new GoodArrayCase([1], Expected: true) },
            { new GoodArrayCase([5], Expected: false) },
            { new GoodArrayCase([1, 1], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsGoodArrayBySubtractionGcd_LeetCodeExamples_ReportsWhetherTheArrayGcdIsOne(
        GoodArrayCase example) =>
        Assert.Equal(example.Expected, CheckIfItIsAGoodArraySolution.IsGoodArrayBySubtractionGcd(example.Nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsGoodArrayByEuclideanGcd_LeetCodeExamples_ReportsWhetherTheArrayGcdIsOne(
        GoodArrayCase example) =>
        Assert.Equal(example.Expected, CheckIfItIsAGoodArraySolution.IsGoodArrayByEuclideanGcd(example.Nums));

    // One LeetCode example: the array under test and whether its overall gcd is one.
    // The expected value is named at every construction site, so a row reads as the
    // case it is rather than as a bare `true` whose meaning is its position. Nested
    // because it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct GoodArrayCase(int[] Nums, bool Expected);
}
