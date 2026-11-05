using DSAExperimentation.LeetCode.Solutions.MaximumProductDifferenceBetweenTwoPairs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumProductDifferenceBetweenTwoPairs;

// Harness only. Both strategies are MaximumProductDifferenceBetweenTwoPairsSolution's
// - the shortest array the constraints
// allow, an all-equal array whose difference is zero, and a case whose extremes sit
// at both ends of the input rather than in sorted order already.
public sealed partial class MaximumProductDifferenceBetweenTwoPairsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [4, 4, 4, 4], 0 },
            { [10, 2, 5, 2], 46 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProductDifferenceByBruteForcePairScan_Cases_ReturnsMaximizedDifference(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            MaximumProductDifferenceBetweenTwoPairsSolution.MaxProductDifferenceByBruteForcePairScan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxProductDifferenceByMergeSortExtremes_Cases_ReturnsMaximizedDifference(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            MaximumProductDifferenceBetweenTwoPairsSolution.MaxProductDifferenceByMergeSortExtremes(nums));
}
