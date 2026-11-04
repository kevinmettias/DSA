using DSAExperimentation.LeetCode.Solutions.MinimumSwapsToMakeSequencesIncreasing;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumSwapsToMakeSequencesIncreasing;

public sealed partial class MinimumSwapsToMakeSequencesIncreasingPromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [1, 3, 5, 4], [1, 2, 3, 7], true },
            { [0, 3, 5, 8, 9], [2, 1, 4, 6, 9], true },
            { [10, 19, 14, 6, 7], [6, 5, 8, 11, 10], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenSomeSwapsSortBoth(int[] nums1, int[] nums2, bool expected)
        => Assert.Equal(expected, MinimumSwapsToMakeSequencesIncreasingPromise.IsKeptBy(nums1, nums2));
}
