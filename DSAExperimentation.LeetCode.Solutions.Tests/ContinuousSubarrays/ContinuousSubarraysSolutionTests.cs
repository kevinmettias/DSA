using DSAExperimentation.LeetCode.Solutions.ContinuousSubarrays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ContinuousSubarrays;

// Harness only. Both strategies are ContinuousSubarraysSolution's - including the
// brute-force rescan, which the benchmark used to own privately as its baseline and
// nothing asserted. The cases pin the window's boundaries: a single element, a
// strictly descending run where every pair is within the limit so every subarray
// counts, a spike that empties the window down to one element, and an alternating run
// where the left edge advances on every step. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class ContinuousSubarraysSolutionTests
{
    public static TheoryData<int[], long> Cases =>
        new()
        {
            // One element: the single subarray is trivially continuous.
            { [7], 1L },

            // Strictly descending by one: every pair is within the limit, so all six count.
            { [3, 2, 1], 6L },

            // A spike far outside the limit - the window collapses to it and restarts.
            { [1, 2, 100, 2, 1], 7L },

            // Every adjacent pair already exceeds the limit, so only singletons count.
            { [1, 10, 1, 10], 4L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountContinuousSubarraysByBruteForceWindows_Cases_ReturnsContinuousSubarrayCount(
        int[] nums, long expected) =>
        Assert.Equal(expected, ContinuousSubarraysSolution.CountContinuousSubarraysByBruteForceWindows(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountContinuousSubarraysByMonotonicDeques_Cases_ReturnsContinuousSubarrayCount(
        int[] nums, long expected) =>
        Assert.Equal(expected, ContinuousSubarraysSolution.CountContinuousSubarraysByMonotonicDeques(nums));
}
