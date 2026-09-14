using DSAExperimentation.LeetCode.MinimumDeletionsToMakeArrayDivisible;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumDeletionsToMakeArrayDivisible;

// Harness only: both strategies live in MinimumDeletionsToMakeArrayDivisibleSolution and are
// asserted here under their own names, so a failure names the strategy that broke. The cases
// cover the three answers the problem can produce - some deletions, none, and -1 when no
// element ever divides - plus a duplicate smallest that must be counted once, not twice.
public sealed class MinimumDeletionsToMakeArrayDivisibleTests
{
    public static TheoryData<int[], int[], int> Examples => new()
    {
        { [2, 3, 2, 4, 3], [9, 6, 9, 3, 15], 2 },
        { [4, 3, 6], [8, 2, 6, 10], -1 },
        { [4, 2, 8], [16, 8, 32], 0 },
        { [3, 3, 2], [9, 9], 1 },
        { [10], [15, 25], -1 },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDeletionsByCandidateScan_Example_ReturnsFewestDeletions(int[] nums, int[] numsDivide, int expected)
    {
        Assert.Equal(expected, MinimumDeletionsToMakeArrayDivisibleSolution.MinDeletionsByCandidateScan(nums, numsDivide));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDeletionsByMergeSort_Example_ReturnsFewestDeletions(int[] nums, int[] numsDivide, int expected)
    {
        Assert.Equal(expected, MinimumDeletionsToMakeArrayDivisibleSolution.MinDeletionsByMergeSort(nums, numsDivide));
    }
}
