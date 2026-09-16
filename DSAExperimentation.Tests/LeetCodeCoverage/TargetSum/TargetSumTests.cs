using DSAExperimentation.LeetCode.TargetSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TargetSum;

// Harness only. Both strategies are TargetSumSolution's - this file just pins them
// to LeetCode's published examples, including the unmemoized recursion baseline that
// was never asserted anywhere before this migration.
public sealed partial class TargetSumTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 1, 1, 1, 1], 3, 5 },
            { [1], 1, 1 },
            { [1, 2], 100, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WaysByUnmemoizedRecursion_LeetCodeExamples_ReturnsSignAssignmentCount(
        int[] nums, int target, int expected)
    {
        var actual = TargetSumSolution.WaysByUnmemoizedRecursion(nums, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void WaysByMemoizedRecursion_LeetCodeExamples_ReturnsSignAssignmentCount(
        int[] nums, int target, int expected)
    {
        var actual = TargetSumSolution.WaysByMemoizedRecursion(nums, target);

        Assert.Equal(expected, actual);
    }
}
