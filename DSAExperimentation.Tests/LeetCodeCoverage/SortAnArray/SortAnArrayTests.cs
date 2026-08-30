using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortAnArray;

// LeetCode 912. Sort an Array: this problem *is* this repo's own
// Algorithms.Sorting.MergeSort.Sort<int, ArrayIndexedSequence<int>> - an O(n log n)
// comparison sort over the array wrapped as an IIndexedSequence<int>, no additional
// glue logic needed beyond wrapping the input.
public sealed partial class SortAnArrayTests
{
    [Theory]
    [InlineData(new[] { 5, 2, 3, 1 }, new[] { 1, 2, 3, 5 })]
    [InlineData(new[] { 5, 1, 1, 2, 0, 0 }, new[] { 0, 0, 1, 1, 2, 5 })]
    [InlineData(new[] { -4, -1, 0, 3, 3, -2 }, new[] { -4, -2, -1, 0, 3, 3 })]
    [InlineData(new[] { 1 }, new[] { 1 })]
    public void SortArray_LeetCodeExamples_ReturnsAscendingOrder(int[] nums, int[] expected)
        => Assert.Equal(expected, SortArray(nums));

    private static int[] SortArray(int[] nums)
    {
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(nums));
        return nums;
    }
}
