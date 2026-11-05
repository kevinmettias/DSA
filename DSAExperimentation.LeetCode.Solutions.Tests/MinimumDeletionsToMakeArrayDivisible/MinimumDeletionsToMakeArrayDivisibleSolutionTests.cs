using DSAExperimentation.LeetCode.Solutions.MinimumDeletionsToMakeArrayDivisible;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumDeletionsToMakeArrayDivisible;

// Harness only: both strategies live in MinimumDeletionsToMakeArrayDivisibleSolution and are
// asserted here under their own names, so a failure names the strategy that broke. The cases
// cover the three answers the problem can produce - some deletions, none, and -1 when no
// element ever divides - plus a duplicate smallest that must be counted once, not twice.
public sealed partial class MinimumDeletionsToMakeArrayDivisibleSolutionTests
{
    public static TheoryData<int[], int[], int> Cases => new()
    {
        { [4, 2, 8], [16, 8, 32], 0 },
        { [3, 3, 2], [9, 9], 1 },
        { [10], [15, 25], -1 },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinDeletionsByCandidateScan_Example_ReturnsFewestDeletions(int[] nums, int[] numsDivide, int expected)
    {
        var actual = MinimumDeletionsToMakeArrayDivisibleSolution.MinDeletionsByCandidateScan(nums, numsDivide);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinDeletionsByMergeSort_Example_ReturnsFewestDeletions(int[] nums, int[] numsDivide, int expected)
    {
        var actual = MinimumDeletionsToMakeArrayDivisibleSolution.MinDeletionsByMergeSort(nums, numsDivide);

        Assert.Equal(expected, actual);
    }
}
