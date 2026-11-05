using DSAExperimentation.LeetCode.Solutions.MaximumGap;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumGap;

// Harness only. Both strategies are MaximumGapSolution's - this file just pins
// them to an all-duplicates edge case and a two-element case with a large gap.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MaximumGapSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 1, 1, 1], 0 },
            { [1, 10_000_000], 9_999_999 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumGapBySelectionSort_Cases_ReturnsLargestSortedNeighborGap(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumGapSolution.MaximumGapBySelectionSort(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumGapByMergeSort_Cases_ReturnsLargestSortedNeighborGap(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumGapSolution.MaximumGapByMergeSort(nums));
}
