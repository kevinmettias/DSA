using DSAExperimentation.LeetCode.SearchInsertPosition;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInsertPosition;

public sealed partial class SearchInsertPositionTests
{
    public static TheoryData<int[], int, int> Examples => new()
    {
        { new[] { 1, 3, 5, 6 }, 5, 2 },
        { new[] { 1, 3, 5, 6 }, 2, 1 },
        { new[] { 1, 3, 5, 6 }, 7, 4 },
        { new[] { 1, 3, 5, 6 }, 0, 0 },
        { Array.Empty<int>(), 0, 0 },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchInsertByLinearScan_LeetCodeExamples_ReturnsLowerBoundIndex(
        int[] nums, int target, int expected)
    {
        var actual = SearchInsertPositionSolution.SearchInsertByLinearScan(nums, target);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SearchInsertByBinarySearchLowerBound_LeetCodeExamples_ReturnsLowerBoundIndex(
        int[] nums, int target, int expected)
    {
        var actual = SearchInsertPositionSolution.SearchInsertByBinarySearchLowerBound(nums, target);
        Assert.Equal(expected, actual);
    }
}
