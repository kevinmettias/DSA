using DSAExperimentation.LeetCode.Solutions.LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit;

// Harness only. Both the O(n^2) rescan and the double-monotonic-deque window live in
// LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution; this file
// pins them to the cases the original test
// carried, so a disagreement names the strategy that broke.
public sealed partial class LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            // Whole array inside the limit - the max deque never has to shed a front.
            { [4, 8, 5, 1, 7, 9], 9, 6 },

            // Every adjacent pair violates the limit, so no window ever grows.
            { [1, 100, 1, 100], 0, 1 },

            // Single element.
            { [5], 0, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestSubarrayByBruteForceWindows_Cases_ReturnsLongestWindowWithinLimit(
        int[] nums, int limit, int expected)
    {
        var actual = LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution
            .LongestSubarrayByBruteForceWindows(nums, limit);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LongestSubarrayByMonotonicDeques_Cases_ReturnsLongestWindowWithinLimit(
        int[] nums, int limit, int expected)
    {
        var actual = LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution
            .LongestSubarrayByMonotonicDeques(nums, limit);

        Assert.Equal(expected, actual);
    }
}
