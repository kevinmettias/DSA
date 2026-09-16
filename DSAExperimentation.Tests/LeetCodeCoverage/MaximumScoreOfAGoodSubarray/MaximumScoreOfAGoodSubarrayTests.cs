using DSAExperimentation.LeetCode.MaximumScoreOfAGoodSubarray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumScoreOfAGoodSubarray;

// Harness only: both strategies live in MaximumScoreOfAGoodSubarraySolution. The
// O(n^2) expand baseline used to exist only as a benchmark arm with nothing
// asserting it, so it is pinned to the same examples as the monotonic-stack
// sweep here.
public sealed partial class MaximumScoreOfAGoodSubarrayTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            // LeetCode's two published examples.
            { [1, 4, 3, 7, 4, 5], 3, 15 },
            { [5, 5, 4, 5, 4, 1, 1, 1], 0, 20 },

            // A single-element array: the only good subarray is the one element at
            // the required index.
            { [7], 0, 7 },

            // A plateau either side of the required index, so the ties the two
            // sweeps break differently (>= popping in both directions) still yield
            // one window.
            { [6, 5, 6], 1, 15 },

            // Strictly decreasing with the required index at the far end: every
            // window containing it has the same minimum, so the widest one wins.
            { [4, 3, 2, 1], 3, 4 },

            // The required index at the far end of a strictly increasing run, where
            // the best trade between minimum and width is an interior window rather
            // than either extreme.
            { [1, 2, 3, 4], 3, 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumScoreByBruteForceExpand_LeetCodeExamples_ReturnsBestGoodSubarrayScore(
        int[] nums, int requiredIndex, int expected)
    {
        var actual = MaximumScoreOfAGoodSubarraySolution.MaximumScoreByBruteForceExpand(nums, requiredIndex);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumScoreByMonotonicStackBoundaries_LeetCodeExamples_ReturnsBestGoodSubarrayScore(
        int[] nums, int requiredIndex, int expected)
    {
        var actual = MaximumScoreOfAGoodSubarraySolution.MaximumScoreByMonotonicStackBoundaries(
            nums, requiredIndex);

        Assert.Equal(expected, actual);
    }
}
