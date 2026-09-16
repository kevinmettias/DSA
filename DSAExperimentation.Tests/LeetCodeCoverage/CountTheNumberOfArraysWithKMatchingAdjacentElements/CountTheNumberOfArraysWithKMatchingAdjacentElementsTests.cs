using DSAExperimentation.LeetCode.CountTheNumberOfArraysWithKMatchingAdjacentElements;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheNumberOfArraysWithKMatchingAdjacentElements;

// Harness only: both strategies are
// CountTheNumberOfArraysWithKMatchingAdjacentElementsSolution's. One test method
// per strategy over LeetCode's own examples, so a failure names the strategy
// that broke.
public sealed class CountTheNumberOfArraysWithKMatchingAdjacentElementsTests
{
    public static TheoryData<int, int, int, long> Examples =>
        new()
        {
            { 3, 2, 1, 4 },
            { 4, 2, 2, 6 },
            { 5, 2, 0, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodArraysByBruteForce_LeetCodeExamples_ReturnsGoodArrayCount(
        int arrayLength, int maxValue, int matchCount, long expected)
    {
        var actual = CountTheNumberOfArraysWithKMatchingAdjacentElementsSolution.CountGoodArraysByBruteForce(
            arrayLength, maxValue, matchCount);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodArraysByModularCombinatorics_LeetCodeExamples_ReturnsGoodArrayCount(
        int arrayLength, int maxValue, int matchCount, long expected)
    {
        var actual = CountTheNumberOfArraysWithKMatchingAdjacentElementsSolution.CountGoodArraysByModularCombinatorics(
            arrayLength, maxValue, matchCount);
        Assert.Equal(expected, actual);
    }
}
