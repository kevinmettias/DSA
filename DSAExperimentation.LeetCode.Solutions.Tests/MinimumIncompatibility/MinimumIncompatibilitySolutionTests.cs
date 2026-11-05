using DSAExperimentation.LeetCode.Solutions.MinimumIncompatibility;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumIncompatibility;

// Harness only: both strategies live in MinimumIncompatibilitySolution and are
// asserted against the same cases - LeetCode's own three, the `groupCount` = 1
// degenerate case where the whole array is one group, and a `groupCount` = n case
// where every group is a single element so repeated values are no obstacle at all.
public sealed partial class MinimumIncompatibilitySolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [1, 2, 3, 4], 1, 3 },
            { [1, 1], 2, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumIncompatibilityByUnmemoizedRecursion_Cases_ReturnsMinimalTotalIncompatibility(
        int[] nums, int groupCount, int expected)
    {
        var actual = MinimumIncompatibilitySolution.MinimumIncompatibilityByUnmemoizedRecursion(nums, groupCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumIncompatibilityByMemoizedRecursion_Cases_ReturnsMinimalTotalIncompatibility(
        int[] nums, int groupCount, int expected)
    {
        var actual = MinimumIncompatibilitySolution.MinimumIncompatibilityByMemoizedRecursion(nums, groupCount);

        Assert.Equal(expected, actual);
    }
}
