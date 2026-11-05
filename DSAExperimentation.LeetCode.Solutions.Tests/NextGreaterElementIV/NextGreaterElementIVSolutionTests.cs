using DSAExperimentation.LeetCode.Solutions.NextGreaterElementIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NextGreaterElementIV;

// Harness only: both strategies live in NextGreaterElementIVSolution and are
// asserted against the same cases, among them the equal-values case (strictly
// greater, so ties never count), a strictly decreasing run where nothing resolves,
// and a duplicate-heavy run that exercises the promotion between the two stacks.
public sealed partial class NextGreaterElementIVSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            // Every later value is greater, so index i's second greater is nums[i+2].
            { [1, 2, 3, 4], [3, 4, -1, -1] },

            // A single element has nothing after it.
            { [7], [-1] },

            // Duplicates ahead count separately once they are strictly greater, and
            // the two 1s are both promoted out of the first stack by the same 2.
            { [1, 1, 2, 3, 2], [3, 3, -1, -1, -1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SecondGreaterElementByBruteForce_Cases_ReturnsSecondGreaterPerIndex(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, NextGreaterElementIVSolution.SecondGreaterElementByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SecondGreaterElementByTwoMonotonicStacks_Cases_ReturnsSecondGreaterPerIndex(
        int[] nums, int[] expected) =>
        Assert.Equal(expected, NextGreaterElementIVSolution.SecondGreaterElementByTwoMonotonicStacks(nums));
}
