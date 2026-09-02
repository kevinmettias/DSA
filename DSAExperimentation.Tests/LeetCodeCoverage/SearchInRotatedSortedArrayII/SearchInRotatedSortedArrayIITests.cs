using DSAExperimentation.LeetCode.SearchInRotatedSortedArrayII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInRotatedSortedArrayII;

// Harness only: the algorithms live in SearchInRotatedSortedArrayIISolution. One
// test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke.
public sealed class SearchInRotatedSortedArrayIITests
{
    public static TheoryData<int[], int, bool> Examples =>
        new()
        {
            { [2, 5, 6, 0, 0, 1, 2], 0, true },
            { [2, 5, 6, 0, 0, 1, 2], 3, false },
            { [1, 0, 1, 1, 1], 0, true },
            { [1, 1, 1, 1, 1], 2, false },
            { [], 5, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchByLinearScan_LeetCodeExamples_ReturnsWhetherTargetIsPresent(int[] nums, int target, bool expected) =>
        Assert.Equal(expected, SearchInRotatedSortedArrayIISolution.SearchByLinearScan(nums, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchByTrimDuplicatesThenBinarySearch_LeetCodeExamples_ReturnsWhetherTargetIsPresent(
        int[] nums, int target, bool expected) =>
        Assert.Equal(expected, SearchInRotatedSortedArrayIISolution.SearchByTrimDuplicatesThenBinarySearch(nums, target));
}
