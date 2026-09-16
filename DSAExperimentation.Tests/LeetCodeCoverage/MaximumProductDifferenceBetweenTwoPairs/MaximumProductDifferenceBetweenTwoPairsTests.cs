using DSAExperimentation.LeetCode.MaximumProductDifferenceBetweenTwoPairs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProductDifferenceBetweenTwoPairs;

// Harness only. Both strategies are MaximumProductDifferenceBetweenTwoPairsSolution's
// - LeetCode's two published examples plus the shortest array the constraints
// allow, an all-equal array whose difference is zero, and a case whose extremes sit
// at both ends of the input rather than in sorted order already.
public sealed partial class MaximumProductDifferenceBetweenTwoPairsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [5, 6, 2, 7, 4], 34 },
            { [4, 2, 5, 9, 7, 4, 8], 64 },
            { [1, 2, 3, 4], 10 },
            { [4, 4, 4, 4], 0 },
            { [10, 2, 5, 2], 46 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProductDifferenceByBruteForcePairScan_LeetCodeExamples_ReturnsMaximizedDifference(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            MaximumProductDifferenceBetweenTwoPairsSolution.MaxProductDifferenceByBruteForcePairScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProductDifferenceByMergeSortExtremes_LeetCodeExamples_ReturnsMaximizedDifference(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            MaximumProductDifferenceBetweenTwoPairsSolution.MaxProductDifferenceByMergeSortExtremes(nums));
}
