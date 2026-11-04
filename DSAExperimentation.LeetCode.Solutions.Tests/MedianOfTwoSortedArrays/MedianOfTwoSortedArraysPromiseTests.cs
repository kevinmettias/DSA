using DSAExperimentation.LeetCode.Solutions.MedianOfTwoSortedArrays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MedianOfTwoSortedArrays;

public sealed partial class MedianOfTwoSortedArraysPromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [1, 3], [2], true },
            { [], [1], true },
            { [-1, 3, 5, 7, 9], [-2, -4, -6, -8, -10], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForTwoSortedArrays(int[] nums1, int[] nums2, bool expected)
        => Assert.Equal(expected, MedianOfTwoSortedArraysPromise.IsKeptBy(nums1, nums2));
}
