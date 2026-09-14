using DSAExperimentation.LeetCode.NextGreaterElementIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NextGreaterElementIV;

// Harness only: both strategies live in NextGreaterElementIVSolution and are
// asserted against the same examples, including the equal-values case (strictly
// greater, so ties never count), a strictly decreasing run where nothing resolves,
// and a duplicate-heavy run that exercises the promotion between the two stacks.
public sealed class NextGreaterElementIVTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            // LeetCode example 1.
            { [2, 4, 0, 9, 6], [9, 6, 6, -1, -1] },

            // LeetCode example 2: equal values are not greater.
            { [3, 3], [-1, -1] },

            // Every later value is greater, so index i's second greater is nums[i+2].
            { [1, 2, 3, 4], [3, 4, -1, -1] },

            // Strictly decreasing: nothing after any index is greater at all.
            { [5, 4, 3, 2, 1], [-1, -1, -1, -1, -1] },

            // A single element has nothing after it.
            { [7], [-1] },

            // Duplicates ahead count separately once they are strictly greater, and
            // the two 1s are both promoted out of the first stack by the same 2.
            { [1, 1, 2, 3, 2], [3, 3, -1, -1, -1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SecondGreaterElementByBruteForce_LeetCodeExamples_ReturnsSecondGreaterPerIndex(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, NextGreaterElementIVSolution.SecondGreaterElementByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SecondGreaterElementByTwoMonotonicStacks_LeetCodeExamples_ReturnsSecondGreaterPerIndex(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, NextGreaterElementIVSolution.SecondGreaterElementByTwoMonotonicStacks(nums));
}
