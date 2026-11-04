using DSAExperimentation.LeetCode.Solutions.SearchInRotatedSortedArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SearchInRotatedSortedArray;

public sealed partial class SearchInRotatedSortedArrayPromiseTests
{
    public static TheoryData<int[], int, bool> Inputs =>
        new()
        {
            { [4, 5, 6, 7, 0, 1, 2], 0, true },
            { [1], 0, true },
            { [2, 5, 6, 0, 1, 3, 4], 1, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForARotatedIncreasingArray(int[] nums, int target, bool expected)
        => Assert.Equal(expected, SearchInRotatedSortedArrayPromise.IsKeptBy(nums, target));
}
