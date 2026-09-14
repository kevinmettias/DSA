using DSAExperimentation.LeetCode.SubarraysDistinctElementSumOfSquaresI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubarraysDistinctElementSumOfSquaresI;

// Harness only: the algorithms live in SubarraysDistinctElementSumOfSquaresISolution.
// One test method per strategy over one shared set of examples, so a failure names
// the strategy that broke. LeetCode's two published examples are joined by a
// singleton and an all-distinct array, which the original test - asserting one
// inlined helper against two cases - never covered.
public sealed class SubarraysDistinctElementSumOfSquaresITests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [1, 2, 1], 15 },
            { [1, 1], 3 },
            { [2, 2], 3 },
            { [1], 1 },
            { [1, 2, 3], 20 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfSquaresByBruteForce_LeetCodeExamples_ReturnsSumOfSquaredDistinctCounts(
        int[] nums, long expected) =>
        Assert.Equal(expected, SubarraysDistinctElementSumOfSquaresISolution.SumOfSquaresByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumOfSquaresByGrowingSet_LeetCodeExamples_ReturnsSumOfSquaredDistinctCounts(
        int[] nums, long expected) =>
        Assert.Equal(expected, SubarraysDistinctElementSumOfSquaresISolution.SumOfSquaresByGrowingSet(nums));
}
