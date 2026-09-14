using DSAExperimentation.LeetCode.ContinuousSubarrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContinuousSubarrays;

// Harness only. Both strategies are ContinuousSubarraysSolution's - including the
// brute-force rescan, which the benchmark used to own privately as its baseline and
// nothing asserted. Beyond LeetCode's two published examples the cases pin the
// window's boundaries: a single element, a run where every pair is within the limit
// so every subarray counts, a strictly descending run where the left edge advances on
// almost every step, and a spike that empties the window down to one element.
public sealed class ContinuousSubarraysTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            // LC example 1.
            { [5, 4, 2, 4], 8L },

            // LC example 2: every pair is within 2, so all six subarrays count.
            { [1, 2, 3], 6L },

            // All values equal: every one of the ten subarrays counts.
            { [1, 1, 1, 1], 10L },

            // One element: the single subarray is trivially continuous.
            { [7], 1L },

            // Strictly descending by one, same shape as the ascending example.
            { [3, 2, 1], 6L },

            // A spike far outside the limit - the window collapses to it and restarts.
            { [1, 2, 100, 2, 1], 7L },

            // Every adjacent pair already exceeds the limit, so only singletons count.
            { [1, 10, 1, 10], 4L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountContinuousSubarraysByBruteForceWindows_LeetCodeExamples_ReturnsContinuousSubarrayCount(
        int[] nums, long expected) =>
        Assert.Equal(expected, ContinuousSubarraysSolution.CountContinuousSubarraysByBruteForceWindows(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountContinuousSubarraysByMonotonicDeques_LeetCodeExamples_ReturnsContinuousSubarrayCount(
        int[] nums, long expected) =>
        Assert.Equal(expected, ContinuousSubarraysSolution.CountContinuousSubarraysByMonotonicDeques(nums));
}
