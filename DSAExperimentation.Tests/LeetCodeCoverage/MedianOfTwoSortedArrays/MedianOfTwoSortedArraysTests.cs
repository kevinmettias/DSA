using DSAExperimentation.LeetCode.MedianOfTwoSortedArrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MedianOfTwoSortedArrays;

// Harness only. Both strategies are MedianOfTwoSortedArraysSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class MedianOfTwoSortedArraysTests
{
    public static TheoryData<int[], int[], double> Examples =>
        new()
        {
            { [1, 3], [2], 2 },
            { [1, 2], [3, 4], 2.5 },
            { [], [1, 2, 3], 2 },
            { [1, 2, 3], [4, 5, 6], 3.5 },
            { [1], [2], 1.5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMedianByMergeAndSort_LeetCodeExamples_ReturnsMedian(
        int[] nums1, int[] nums2, double expected) =>
        Assert.Equal(expected, MedianOfTwoSortedArraysSolution.FindMedianByMergeAndSort(nums1, nums2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMedianByBinarySearchPartition_LeetCodeExamples_ReturnsMedian(
        int[] nums1, int[] nums2, double expected) =>
        Assert.Equal(expected, MedianOfTwoSortedArraysSolution.FindMedianByBinarySearchPartition(nums1, nums2));
}
