using DSAExperimentation.LeetCode.Solutions.SortAnArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SortAnArray;

// Harness only. Both sorts are SortAnArraySolution's; this file pins them to
// the shapes a sort has to survive - a
// singleton, an already-sorted run, a strict reversal, and an all-equal run.
public sealed partial class SortAnArraySolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [-4, -1, 0, 3, 3, -2], [-4, -2, -1, 0, 3, 3] },
            { [1, 2, 3, 4, 5], [1, 2, 3, 4, 5] },
            { [5, 4, 3, 2, 1], [1, 2, 3, 4, 5] },
            { [7, 7, 7, 7], [7, 7, 7, 7] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SortArrayByInsertionSort_Cases_ReturnsAscendingOrder(int[] nums, int[] expected) =>
        Assert.Equal(expected, SortAnArraySolution.SortArrayByInsertionSort(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SortArrayByMergeSort_Cases_ReturnsAscendingOrder(int[] nums, int[] expected) =>
        Assert.Equal(expected, SortAnArraySolution.SortArrayByMergeSort(nums));
}
