using DSAExperimentation.LeetCode.SelectCellsInGridWithMaximumScore;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SelectCellsInGridWithMaximumScore;

// Harness only: both strategies are SelectCellsInGridWithMaximumScoreSolution's -
// this file just pins them to LeetCode's published examples.
public sealed partial class SelectCellsInGridWithMaximumScoreTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 2, 3], [4, 3, 2], [1, 1, 1]], 8 },
            { [[8, 7, 6], [8, 3, 2]], 15 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxScoreByBruteForceRecursion_LeetCodeExamples_ReturnsMaximumOneCellPerRowDistinctValueSum(
        int[][] grid, int expected) =>
        Assert.Equal(expected, SelectCellsInGridWithMaximumScoreSolution.MaxScoreByBruteForceRecursion(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxScoreByBitmaskMemoization_LeetCodeExamples_ReturnsMaximumOneCellPerRowDistinctValueSum(
        int[][] grid, int expected) =>
        Assert.Equal(expected, SelectCellsInGridWithMaximumScoreSolution.MaxScoreByBitmaskMemoization(grid));

    // LeetCode example 1, whose last row repeats one value: a row is listed once per value
    // it holds, however often it holds it, because the strategy may take one cell per row.
    [Fact]
    public void GroupRowsByValue_RowRepeatingAValue_ListsThatRowOnceForIt()
    {
        var rowsByValue = SelectCellsInGridWithMaximumScoreSolution.GroupRowsByValue([[1, 2, 3], [4, 3, 2], [1, 1, 1]]);

        Assert.Equal(
            ["1:0,2", "2:0,1", "3:0,1", "4:1"],
            rowsByValue.OrderBy(entry => entry.Key).Select(entry => $"{entry.Key}:{string.Join(',', entry.Value)}"));
    }
}
