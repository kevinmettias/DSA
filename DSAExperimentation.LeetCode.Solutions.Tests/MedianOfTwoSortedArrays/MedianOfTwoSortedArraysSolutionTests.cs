using DSAExperimentation.LeetCode.Solutions.MedianOfTwoSortedArrays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MedianOfTwoSortedArrays;

// Harness only. Both strategies are MedianOfTwoSortedArraysSolution's - this file
// just pins them to an empty first array, two arrays whose ranges do not overlap,
// and two single-element arrays. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MedianOfTwoSortedArraysSolutionTests
{
    public static TheoryData<int[], int[], double> Cases =>
        new()
        {
            { [], [1, 2, 3], 2 },
            { [1, 2, 3], [4, 5, 6], 3.5 },
            { [1], [2], 1.5 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMedianByMergeAndSort_Cases_ReturnsMedian(
        int[] nums1, int[] nums2, double expected)
    {
        var actual = MedianOfTwoSortedArraysSolution.FindMedianByMergeAndSort(nums1, nums2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMedianByBinarySearchPartition_Cases_ReturnsMedian(
        int[] nums1, int[] nums2, double expected)
    {
        var actual = MedianOfTwoSortedArraysSolution.FindMedianByBinarySearchPartition(nums1, nums2);

        Assert.Equal(expected, actual);
    }
}
