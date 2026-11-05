using DSAExperimentation.LeetCode.Solutions.MinimumSwapsToMakeSequencesIncreasing;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumSwapsToMakeSequencesIncreasing;

// Harness only. Both strategies are MinimumSwapsToMakeSequencesIncreasingSolution's -
// this file pins them to the degenerate shapes the
// recurrence's base case has to survive: a single index, a pair already increasing in
// both arrays, and a pair that only becomes increasing by swapping the last index.
public sealed partial class MinimumSwapsToMakeSequencesIncreasingSolutionTests
{
    public static TheoryData<int[], int[], int> Cases =>
        new()
        {
            { [1], [1], 0 },
            { [1, 2, 3], [4, 5, 6], 0 },
            { [1, 2, 3, 4, 5], [0, 1, 2, 3, 4], 0 },
            { [1, 1], [0, 2], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSwapByTabulation_Cases_ReturnsMinimumSwapCount(
        int[] nums1, int[] nums2, int expected)
    {
        var actual = MinimumSwapsToMakeSequencesIncreasingSolution.MinSwapByTabulation(nums1, nums2);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinSwapByMemoizedTwoState_Cases_ReturnsMinimumSwapCount(
        int[] nums1, int[] nums2, int expected)
    {
        var actual = MinimumSwapsToMakeSequencesIncreasingSolution.MinSwapByMemoizedTwoState(nums1, nums2);
        Assert.Equal(expected, actual);
    }
}
