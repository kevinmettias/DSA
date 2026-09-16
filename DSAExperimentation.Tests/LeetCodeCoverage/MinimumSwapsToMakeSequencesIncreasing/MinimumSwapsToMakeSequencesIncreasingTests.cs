using DSAExperimentation.LeetCode.MinimumSwapsToMakeSequencesIncreasing;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumSwapsToMakeSequencesIncreasing;

// Harness only. Both strategies are MinimumSwapsToMakeSequencesIncreasingSolution's -
// this file pins them to LeetCode's published examples plus the degenerate shapes the
// recurrence's base case has to survive: a single index, a pair already increasing in
// both arrays, and a pair that only becomes increasing by swapping the last index.
public sealed partial class MinimumSwapsToMakeSequencesIncreasingTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [1, 3, 5, 4], [1, 2, 3, 7], 1 },
            { [0, 3, 5, 8, 9], [2, 1, 4, 6, 9], 1 },
            { [1], [1], 0 },
            { [1, 2, 3], [4, 5, 6], 0 },
            { [1, 2, 3, 4, 5], [0, 1, 2, 3, 4], 0 },
            { [1, 1], [0, 2], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinSwapByTabulation_LeetCodeExamples_ReturnsMinimumSwapCount(
        int[] nums1, int[] nums2, int expected)
    {
        var actual = MinimumSwapsToMakeSequencesIncreasingSolution.MinSwapByTabulation(nums1, nums2);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinSwapByMemoizedTwoState_LeetCodeExamples_ReturnsMinimumSwapCount(
        int[] nums1, int[] nums2, int expected)
    {
        var actual = MinimumSwapsToMakeSequencesIncreasingSolution.MinSwapByMemoizedTwoState(nums1, nums2);
        Assert.Equal(expected, actual);
    }
}
