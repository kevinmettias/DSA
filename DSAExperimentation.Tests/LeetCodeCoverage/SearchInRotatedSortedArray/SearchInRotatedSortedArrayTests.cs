using DSAExperimentation.LeetCode.SearchInRotatedSortedArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInRotatedSortedArray;

// Harness only: the algorithms live in SearchInRotatedSortedArraySolution. One
// test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed partial class SearchInRotatedSortedArrayTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { new[] { 4, 5, 6, 7, 0, 1, 2 }, 0, 4 },
            { new[] { 4, 5, 6, 7, 0, 1, 2 }, 3, -1 },
            { new[] { 1 }, 0, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchByLinearScan_LeetCodeExamples_ReturnsTargetIndexOrMinusOne(int[] nums, int target, int expected)
    {
        var actual = SearchInRotatedSortedArraySolution.SearchByLinearScan(nums, target);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchByBinarySearchPivotAndSlice_LeetCodeExamples_ReturnsTargetIndexOrMinusOne(
        int[] nums, int target, int expected)
    {
        var actual = SearchInRotatedSortedArraySolution.SearchByBinarySearchPivotAndSlice(nums, target);

        Assert.Equal(expected, actual);
    }
}
