using DSAExperimentation.LeetCode.MaximumGap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumGap;

// Harness only. Both strategies are MaximumGapSolution's - LC 164's published
// examples plus an all-duplicates edge case and a two-element case with a large
// gap - this file just pins them to LeetCode's expected answers.
public sealed partial class MaximumGapTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 6, 9, 1], 3 },
            { [10], 0 },
            { [1, 1, 1, 1], 0 },
            { [1, 10_000_000], 9_999_999 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumGapBySelectionSort_LeetCodeExamples_ReturnsLargestSortedNeighborGap(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumGapSolution.MaximumGapBySelectionSort(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumGapByMergeSort_LeetCodeExamples_ReturnsLargestSortedNeighborGap(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumGapSolution.MaximumGapByMergeSort(nums));
}
