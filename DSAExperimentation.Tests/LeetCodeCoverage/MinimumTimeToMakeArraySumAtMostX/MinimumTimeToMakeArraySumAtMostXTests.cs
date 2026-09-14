using DSAExperimentation.LeetCode.MinimumTimeToMakeArraySumAtMostX;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToMakeArraySumAtMostX;

// Harness only. Both strategies are
// MinimumTimeToMakeArraySumAtMostXSolution's - including the dense table, which
// the benchmark used to own privately as its baseline and nothing asserted.
// Beyond LeetCode's two published examples the cases pin the schedule's
// boundaries: a sum already at or below x (no seconds needed at all), a target
// only the very last operation reaches (every index has to be zeroed), and a
// nums2 of all zeroes, where nothing grows and the reduction is pure nums1.
public sealed class MinimumTimeToMakeArraySumAtMostXTests
{
    public static TheoryData<int[], int[], int, int> Examples =>
        new()
        {
            // LC example 1.
            { [1, 2, 3], [1, 2, 3], 4, 3 },

            // LC example 2: nums2 outgrows every schedule.
            { [1, 2, 3], [3, 3, 3], 4, -1 },

            // Already at or below x before the first second elapses.
            { [5], [1], 10, 0 },

            // A single zeroing is enough.
            { [4, 1], [1, 1], 3, 1 },

            // Nothing grows, so x is reached once enough of nums1 is zeroed.
            { [1, 2], [0, 0], 0, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByDenseTable_LeetCodeExamples_ReturnsFewestSecondsToReachTarget(
        int[] nums1, int[] nums2, int x, int expected) =>
        Assert.Equal(expected, MinimumTimeToMakeArraySumAtMostXSolution.MinimumTimeByDenseTable(nums1, nums2, x));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByRollingKnapsack_LeetCodeExamples_ReturnsFewestSecondsToReachTarget(
        int[] nums1, int[] nums2, int x, int expected) =>
        Assert.Equal(expected, MinimumTimeToMakeArraySumAtMostXSolution.MinimumTimeByRollingKnapsack(nums1, nums2, x));
}
