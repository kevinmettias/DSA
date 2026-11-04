using DSAExperimentation.LeetCode.Solutions.FindMinimumInRotatedSortedArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindMinimumInRotatedSortedArray;

public sealed partial class FindMinimumInRotatedSortedArrayPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [3, 4, 5, 1, 2], true },
            { [11, 13, 15, 17], true },
            { [1], true },
            { [2, 3, 4, 5, 6, 7, 8, 9, 1, 0], false },
            { [1, 1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForARotatedIncreasingArray(int[] nums, bool expected)
        => Assert.Equal(expected, FindMinimumInRotatedSortedArrayPromise.IsKeptBy(nums));
}
