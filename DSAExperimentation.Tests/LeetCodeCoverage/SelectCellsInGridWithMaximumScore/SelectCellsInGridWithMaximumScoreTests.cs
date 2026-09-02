using DSAExperimentation.LeetCode.SelectCellsInGridWithMaximumScore;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SelectCellsInGridWithMaximumScore;

// Harness only: both strategies are SelectCellsInGridWithMaximumScoreSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class SelectCellsInGridWithMaximumScoreTests
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
}
