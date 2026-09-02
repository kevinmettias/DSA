using DSAExperimentation.LeetCode.FindFirstAndLastPositionOfElementInSortedArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindFirstAndLastPositionOfElementInSortedArray;

public sealed class FindFirstAndLastPositionOfElementInSortedArrayTests
{
    public static TheoryData<int[], int, int[]> Examples => new()
    {
        { new[] { 5, 7, 7, 8, 8, 10 }, 8, new[] { 3, 4 } },
        { new[] { 5, 7, 7, 8, 8, 10 }, 6, new[] { -1, -1 } },
        { Array.Empty<int>(), 0, new[] { -1, -1 } },
        { new[] { 1 }, 1, new[] { 0, 0 } },
        { new[] { 2, 2 }, 2, new[] { 0, 1 } },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchRangeByLinearScan_LeetCodeExamples_ReturnsClosedRange(
        int[] nums, int target, int[] expected)
    {
        var actual = FindFirstAndLastPositionOfElementInSortedArraySolution.SearchRangeByLinearScan(nums, target);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchRangeByBinarySearchBounds_LeetCodeExamples_ReturnsClosedRange(
        int[] nums, int target, int[] expected)
    {
        var actual = FindFirstAndLastPositionOfElementInSortedArraySolution.SearchRangeByBinarySearchBounds(nums, target);
        Assert.Equal(expected, actual);
    }
}
