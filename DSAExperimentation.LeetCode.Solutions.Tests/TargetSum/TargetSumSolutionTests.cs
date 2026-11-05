using DSAExperimentation.LeetCode.Solutions.TargetSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TargetSum;

// Harness only. Both strategies are TargetSumSolution's - this file just pins them
// to a target no assignment of signs can reach. The unmemoized recursion baseline
// was never asserted anywhere before this migration. LeetCode's published examples
// run in CatalogCaseTests.
public sealed partial class TargetSumSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [1, 2], 100, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void WaysByUnmemoizedRecursion_Cases_ReturnsSignAssignmentCount(
        int[] nums, int target, int expected)
    {
        var actual = TargetSumSolution.WaysByUnmemoizedRecursion(nums, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void WaysByMemoizedRecursion_Cases_ReturnsSignAssignmentCount(
        int[] nums, int target, int expected)
    {
        var actual = TargetSumSolution.WaysByMemoizedRecursion(nums, target);

        Assert.Equal(expected, actual);
    }
}
