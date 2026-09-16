using DSAExperimentation.LeetCode.SortAnArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortAnArray;

// Harness only. Both sorts are SortAnArraySolution's; this file pins them to
// LeetCode's published examples plus the shapes a sort has to survive - a
// singleton, an already-sorted run, a strict reversal, and an all-equal run.
public sealed partial class SortAnArrayTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [5, 2, 3, 1], [1, 2, 3, 5] },
            { [5, 1, 1, 2, 0, 0], [0, 0, 1, 1, 2, 5] },
            { [-4, -1, 0, 3, 3, -2], [-4, -2, -1, 0, 3, 3] },
            { [1], [1] },
            { [1, 2, 3, 4, 5], [1, 2, 3, 4, 5] },
            { [5, 4, 3, 2, 1], [1, 2, 3, 4, 5] },
            { [7, 7, 7, 7], [7, 7, 7, 7] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortArrayByInsertionSort_LeetCodeExamples_ReturnsAscendingOrder(int[] nums, int[] expected) =>
        Assert.Equal(expected, SortAnArraySolution.SortArrayByInsertionSort(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortArrayByMergeSort_LeetCodeExamples_ReturnsAscendingOrder(int[] nums, int[] expected) =>
        Assert.Equal(expected, SortAnArraySolution.SortArrayByMergeSort(nums));
}
