using DSAExperimentation.LeetCode.FindMinimumInRotatedSortedArrayII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMinimumInRotatedSortedArrayII;

// Harness only: the algorithms live in FindMinimumInRotatedSortedArrayIISolution.
// One test method per strategy over one shared set of LeetCode's own examples, so
// a failure names the strategy that broke.
public sealed class FindMinimumInRotatedSortedArrayIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 3, 5], 1 },
            { [2, 2, 2, 0, 1], 0 },
            { [10, 1, 10, 10, 10], 1 },
            { [1], 1 },
            { [2, 2, 2, 2], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinByLinearScan_LeetCodeExamples_ReturnsMinimum(int[] nums, int expected) =>
        Assert.Equal(expected, FindMinimumInRotatedSortedArrayIISolution.FindMinByLinearScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinByDuplicateTolerantBinaryShrink_LeetCodeExamples_ReturnsMinimum(int[] nums, int expected) =>
        Assert.Equal(expected, FindMinimumInRotatedSortedArrayIISolution.FindMinByDuplicateTolerantBinaryShrink(nums));
}
