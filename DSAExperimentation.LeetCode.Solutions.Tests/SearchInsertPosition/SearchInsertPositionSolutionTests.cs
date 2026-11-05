using DSAExperimentation.LeetCode.Solutions.SearchInsertPosition;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SearchInsertPosition;

public sealed partial class SearchInsertPositionSolutionTests
{
    public static TheoryData<int[], int, int> Cases => new()
    {
        { Array.Empty<int>(), 0, 0 },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SearchInsertByLinearScan_Cases_ReturnsLowerBoundIndex(
        int[] nums, int target, int expected)
    {
        var actual = SearchInsertPositionSolution.SearchInsertByLinearScan(nums, target);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void SearchInsertByBinarySearchLowerBound_Cases_ReturnsLowerBoundIndex(
        int[] nums, int target, int expected)
    {
        var actual = SearchInsertPositionSolution.SearchInsertByBinarySearchLowerBound(nums, target);
        Assert.Equal(expected, actual);
    }
}
