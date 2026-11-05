using DSAExperimentation.LeetCode.Solutions.SubarraysDistinctElementSumOfSquaresI;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SubarraysDistinctElementSumOfSquaresI;

// Harness only: the algorithms live in SubarraysDistinctElementSumOfSquaresISolution.
// One test method per strategy over one shared case, a pair of equal values, so a
// failure names the strategy that broke. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class SubarraysDistinctElementSumOfSquaresISolutionTests
{
    public static TheoryData<int[], long> Cases =>
        new()
        {
            { [2, 2], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumOfSquaresByBruteForce_Cases_ReturnsSumOfSquaredDistinctCounts(
        int[] nums, long expected) =>
        Assert.Equal(expected, SubarraysDistinctElementSumOfSquaresISolution.SumOfSquaresByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SumOfSquaresByGrowingSet_Cases_ReturnsSumOfSquaredDistinctCounts(
        int[] nums, long expected) =>
        Assert.Equal(expected, SubarraysDistinctElementSumOfSquaresISolution.SumOfSquaresByGrowingSet(nums));
}
