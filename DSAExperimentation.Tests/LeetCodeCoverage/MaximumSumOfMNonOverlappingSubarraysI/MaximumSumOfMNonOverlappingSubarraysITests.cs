using DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumOfMNonOverlappingSubarraysI;

// Harness only. Both strategies are MaximumSumOfMNonOverlappingSubarraysISolution's -
// this file just pins them to LeetCode's published examples, including example 4's
// all-negative array, the case that catches an "at most j" DP into wrongly
// returning 0 by skipping every subarray.
public sealed class MaximumSumOfMNonOverlappingSubarraysITests
{
    public static TheoryData<int[], int, int, int, long> Examples =>
        new()
        {
            { [4, 1, -5, 2], 2, 1, 3, 7 },
            { [1, 0, 3, 4], 2, 1, 2, 8 },
            { [-1, 7, -4], 1, 2, 3, 6 },
            { [-3, -4, -1], 2, 1, 2, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumByDynamicProgramming_LeetCodeExamples_ReturnsBestAtMostMSubarraySum(
        int[] nums, int m, int l, int r, long expected) =>
        Assert.Equal(expected, MaximumSumOfMNonOverlappingSubarraysISolution.MaximumSumByDynamicProgramming(nums, m, l, r));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumBySlidingWindowMaximum_LeetCodeExamples_ReturnsBestAtMostMSubarraySum(
        int[] nums, int m, int l, int r, long expected) =>
        Assert.Equal(expected, MaximumSumOfMNonOverlappingSubarraysISolution.MaximumSumBySlidingWindowMaximum(nums, m, l, r));
}
