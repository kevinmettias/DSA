using DSAExperimentation.LeetCode.LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit;

// Harness only. Both the O(n^2) rescan and the double-monotonic-deque window live in
// LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution; this file
// pins them to LeetCode's published examples plus the cases the original test
// carried, so a disagreement names the strategy that broke.
public sealed class LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            // LeetCode example 1.
            { [8, 2, 4, 7], 4, 2 },

            // LeetCode example 2: the window has to slide past the leading 10.
            { [10, 1, 2, 4, 7, 2], 5, 4 },

            // LeetCode example 3: limit 0 asks for the longest constant run.
            { [4, 2, 2, 2, 4, 4, 2, 2], 0, 3 },

            // Whole array inside the limit - the max deque never has to shed a front.
            { [4, 8, 5, 1, 7, 9], 9, 6 },

            // Every adjacent pair violates the limit, so no window ever grows.
            { [1, 100, 1, 100], 0, 1 },

            // Single element.
            { [5], 0, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestSubarrayByBruteForceWindows_LeetCodeExamples_ReturnsLongestWindowWithinLimit(
        int[] nums, int limit, int expected) =>
        Assert.Equal(
            expected,
            LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution
                .LongestSubarrayByBruteForceWindows(nums, limit));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestSubarrayByMonotonicDeques_LeetCodeExamples_ReturnsLongestWindowWithinLimit(
        int[] nums, int limit, int expected) =>
        Assert.Equal(
            expected,
            LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution
                .LongestSubarrayByMonotonicDeques(nums, limit));
}
