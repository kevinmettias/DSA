using DSAExperimentation.LeetCode.MinimumAbsoluteDifferenceBetweenElementsWithConstraint;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAbsoluteDifferenceBetweenElementsWithConstraint;

// Harness only. Both strategies are
// MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolution's - including the
// pair scan, which the benchmark used to own privately as its baseline with nothing
// asserting it. Beyond LeetCode's three published examples the cases pin the index
// constraint's boundaries: x = 0 (an index may pair with itself, so the answer is 0),
// x = n - 1 (exactly one admissible pair), equal values at the admissible distance,
// and an array whose best pair is neither adjacent nor the closest values overall.
public sealed class MinimumAbsoluteDifferenceBetweenElementsWithConstraintTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            // LC example 1: nums[0] and nums[3] are two apart and equal.
            { [4, 3, 2, 4], 2, 0 },

            // LC example 2.
            { [5, 3, 2, 10, 15], 1, 1 },

            // LC example 3: only the pair (0, 3) is admissible.
            { [1, 2, 3, 4], 3, 3 },

            // x = 0 lets an index pair with itself, so nothing beats zero.
            { [1, 5, 3], 0, 0 },

            // The best pair is neither adjacent nor the two closest values overall.
            { [10, 1, 100, 2], 2, 1 },

            // x = n - 1: exactly one admissible pair, the two ends.
            { [5, 4, 3, 2, 1], 4, 4 },

            // Equal values at the only admissible distance.
            { [7, 7], 1, 0 },

            // Two elements, one pair, no choice to make.
            { [1, 10], 1, 9 },

            // Example 1's array at a wider gap - the equal pair still fits.
            { [4, 3, 2, 4], 3, 0 },

            // Far apart in the array, adjacent in value.
            { [100, 1, 1000, 2, 50, 3], 2, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAbsoluteDifferenceByBruteForcePairScan_LeetCodeExamples_ReturnsSmallestConstrainedGap(
        int[] nums, int x, int expected)
    {
        var actual = MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolution
            .MinAbsoluteDifferenceByBruteForcePairScan(nums, x);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAbsoluteDifferenceByBstSlidingWindow_LeetCodeExamples_ReturnsSmallestConstrainedGap(
        int[] nums, int x, int expected)
    {
        var actual = MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolution
            .MinAbsoluteDifferenceByBstSlidingWindow(nums, x);

        Assert.Equal(expected, actual);
    }
}
