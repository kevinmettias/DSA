using DSAExperimentation.LeetCode.MaximumSubarrayXORWithBoundedRange;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSubarrayXORWithBoundedRange;

// Harness only. Both strategies are MaximumSubarrayXORWithBoundedRangeSolution's -
// this file just pins them to hand-checked examples, including one where every
// element falls outside [low, high] so no valid subarray exists at all.
public sealed class MaximumSubarrayXORWithBoundedRangeTests
{
    public static TheoryData<int[], int, int, int> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5], 2, 4, 7 },
            { [4, 2, 15, 9, 6], 1, 10, 15 },
            { [5, 5, 5], 1, 3, 0 },
            { [7], 0, 10, 7 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubarrayXorByBruteForce_LeetCodeExamples_ReturnsMaxBoundedSubarrayXor(
        int[] nums, int low, int high, int expected) =>
        Assert.Equal(expected, MaximumSubarrayXORWithBoundedRangeSolution.MaxSubarrayXorByBruteForce(nums, low, high));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSubarrayXorByBitTrieSegments_LeetCodeExamples_ReturnsMaxBoundedSubarrayXor(
        int[] nums, int low, int high, int expected) =>
        Assert.Equal(
            expected, MaximumSubarrayXORWithBoundedRangeSolution.MaxSubarrayXorByBitTrieSegments(nums, low, high));
}
