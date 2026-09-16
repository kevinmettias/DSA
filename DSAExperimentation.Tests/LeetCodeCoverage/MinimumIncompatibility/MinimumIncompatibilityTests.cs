using DSAExperimentation.LeetCode.MinimumIncompatibility;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumIncompatibility;

// Harness only: both strategies live in MinimumIncompatibilitySolution and are
// asserted against the same examples - LeetCode's own three, the `groupCount` = 1
// degenerate case where the whole array is one group, and a `groupCount` = n case
// where every group is a single element so repeated values are no obstacle at all.
public sealed partial class MinimumIncompatibilityTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 2, 1, 4], 2, 4 },
            { [6, 3, 8, 1, 3, 1, 2, 2], 4, 6 },
            { [5, 3, 3, 6, 3, 3], 3, -1 },
            { [1, 2, 3, 4], 1, 3 },
            { [1, 1], 2, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumIncompatibilityByUnmemoizedRecursion_LeetCodeExamples_ReturnsMinimalTotalIncompatibility(
        int[] nums, int groupCount, int expected)
    {
        var actual = MinimumIncompatibilitySolution.MinimumIncompatibilityByUnmemoizedRecursion(nums, groupCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumIncompatibilityByMemoizedRecursion_LeetCodeExamples_ReturnsMinimalTotalIncompatibility(
        int[] nums, int groupCount, int expected)
    {
        var actual = MinimumIncompatibilitySolution.MinimumIncompatibilityByMemoizedRecursion(nums, groupCount);

        Assert.Equal(expected, actual);
    }
}
