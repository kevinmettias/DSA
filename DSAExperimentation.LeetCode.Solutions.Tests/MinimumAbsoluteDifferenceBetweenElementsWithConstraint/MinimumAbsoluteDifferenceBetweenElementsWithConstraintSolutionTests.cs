using DSAExperimentation.LeetCode.Solutions.MinimumAbsoluteDifferenceBetweenElementsWithConstraint;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumAbsoluteDifferenceBetweenElementsWithConstraint;

// Harness only. Both strategies are
// MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolution's - including the
// pair scan, which the benchmark used to own privately as its baseline with nothing
// asserting it. The cases pin the index constraint's boundaries:
// minimumIndexDistance = 0 (an index may pair with itself, so the answer is 0),
// minimumIndexDistance = n - 1 (exactly one admissible pair), equal values at the
// admissible distance, and an array whose best pair is neither adjacent nor the
// closest values overall. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            // minimumIndexDistance = 0 lets an index pair with itself, so nothing beats zero.
            { [1, 5, 3], 0, 0 },

            // The best pair is neither adjacent nor the two closest values overall.
            { [10, 1, 100, 2], 2, 1 },

            // minimumIndexDistance = n - 1: exactly one admissible pair, the two ends.
            { [5, 4, 3, 2, 1], 4, 4 },

            // Equal values at the only admissible distance.
            { [7, 7], 1, 0 },

            // Two elements, one pair, no choice to make.
            { [1, 10], 1, 9 },

            // LeetCode example 1's array at a wider gap - the equal pair still fits.
            { [4, 3, 2, 4], 3, 0 },

            // Far apart in the array, adjacent in value.
            { [100, 1, 1000, 2, 50, 3], 2, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinAbsoluteDifferenceByBruteForcePairScan_Cases_ReturnsSmallestConstrainedGap(
        int[] nums, int minimumIndexDistance, int expected)
    {
        var actual = MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolution
            .MinAbsoluteDifferenceByBruteForcePairScan(nums, minimumIndexDistance);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinAbsoluteDifferenceByBstSlidingWindow_Cases_ReturnsSmallestConstrainedGap(
        int[] nums, int minimumIndexDistance, int expected)
    {
        var actual = MinimumAbsoluteDifferenceBetweenElementsWithConstraintSolution
            .MinAbsoluteDifferenceByBstSlidingWindow(nums, minimumIndexDistance);

        Assert.Equal(expected, actual);
    }
}
