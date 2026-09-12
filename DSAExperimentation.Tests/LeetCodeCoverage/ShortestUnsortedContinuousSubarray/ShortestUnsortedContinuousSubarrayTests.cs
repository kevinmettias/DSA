using DSAExperimentation.LeetCode.ShortestUnsortedContinuousSubarray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestUnsortedContinuousSubarray;

// Harness only: both strategies live in ShortestUnsortedContinuousSubarraySolution
// and are asserted against the same examples.
public sealed class ShortestUnsortedContinuousSubarrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 6, 4, 8, 10, 9, 15], 5 },
            { [1, 2, 3, 4], 0 },
            { [1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindUnsortedSubarrayBySelectionSortScan_LeetCodeExamples_ReturnsShortestSpanLength(
        int[] nums, int expected) =>
        Assert.Equal(expected, ShortestUnsortedContinuousSubarraySolution.FindUnsortedSubarrayBySelectionSortScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindUnsortedSubarrayByMergeSortScan_LeetCodeExamples_ReturnsShortestSpanLength(
        int[] nums, int expected) =>
        Assert.Equal(expected, ShortestUnsortedContinuousSubarraySolution.FindUnsortedSubarrayByMergeSortScan(nums));
}
