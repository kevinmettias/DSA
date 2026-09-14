using DSAExperimentation.LeetCode.MaximumScoreOfAGoodSubarray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumScoreOfAGoodSubarray;

// Harness only: both strategies live in MaximumScoreOfAGoodSubarraySolution. The
// O(n^2) expand baseline used to exist only as a benchmark arm with nothing
// asserting it, so it is pinned to the same examples as the monotonic-stack
// sweep here.
public sealed class MaximumScoreOfAGoodSubarrayTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            // LeetCode's two published examples.
            { [1, 4, 3, 7, 4, 5], 3, 15 },
            { [5, 5, 4, 5, 4, 1, 1, 1], 0, 20 },

            // A single-element array: the only good subarray is nums[k] itself.
            { [7], 0, 7 },

            // A plateau either side of k, so the ties the two sweeps break
            // differently (>= popping in both directions) still yield one window.
            { [6, 5, 6], 1, 15 },

            // Strictly decreasing with k at the far end: every window containing
            // k has the same minimum, so the widest one wins.
            { [4, 3, 2, 1], 3, 4 },

            // k at the far end of a strictly increasing run, where the best trade
            // between minimum and width is an interior window rather than either
            // extreme.
            { [1, 2, 3, 4], 3, 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumScoreByBruteForceExpand_LeetCodeExamples_ReturnsBestGoodSubarrayScore(
        int[] nums, int k, int expected) =>
        Assert.Equal(
            expected, MaximumScoreOfAGoodSubarraySolution.MaximumScoreByBruteForceExpand(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumScoreByMonotonicStackBoundaries_LeetCodeExamples_ReturnsBestGoodSubarrayScore(
        int[] nums, int k, int expected) =>
        Assert.Equal(
            expected,
            MaximumScoreOfAGoodSubarraySolution.MaximumScoreByMonotonicStackBoundaries(nums, k));
}
