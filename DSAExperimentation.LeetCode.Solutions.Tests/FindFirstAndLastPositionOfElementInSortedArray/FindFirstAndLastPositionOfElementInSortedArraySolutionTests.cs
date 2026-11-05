using DSAExperimentation.LeetCode.Solutions.FindFirstAndLastPositionOfElementInSortedArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindFirstAndLastPositionOfElementInSortedArray;

public sealed partial class FindFirstAndLastPositionOfElementInSortedArraySolutionTests
{
    public static TheoryData<int[], int, int[]> Cases => new()
    {
        { new[] { 2, 2 }, 2, new[] { 0, 1 } },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SearchRangeByLinearScan_Cases_ReturnsClosedRange(
        int[] nums, int target, int[] expected)
    {
        var actual = FindFirstAndLastPositionOfElementInSortedArraySolution.SearchRangeByLinearScan(nums, target);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SearchRangeByBinarySearchBounds_Cases_ReturnsClosedRange(
        int[] nums, int target, int[] expected)
    {
        var actual = FindFirstAndLastPositionOfElementInSortedArraySolution.SearchRangeByBinarySearchBounds(nums, target);
        Assert.Equal(expected, actual);
    }
}
