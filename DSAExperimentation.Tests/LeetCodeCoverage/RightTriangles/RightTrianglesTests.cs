using DSAExperimentation.LeetCode.RightTriangles;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RightTriangles;

// Harness only: both counting strategies live in RightTrianglesSolution. One test
// method per strategy over one shared set of LeetCode's own examples, so a failure
// names the strategy that broke.
public sealed partial class RightTrianglesTests
{
    public static TheoryData<int[][], long> Examples =>
        new()
        {
            { [[0, 1, 0], [0, 1, 1], [0, 1, 0]], 2L },
            { [[1, 0, 0, 0], [0, 1, 0, 1], [1, 0, 0, 0]], 0L },
            { [[1, 0, 1], [1, 0, 0], [1, 0, 0]], 2L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForceRowColumnScan_LeetCodeExamples_ReturnsRightTriangleCount(
        int[][] grid, long expected) =>
        Assert.Equal(expected, RightTrianglesSolution.CountByBruteForceRowColumnScan(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByTalliedRowsAndColumns_LeetCodeExamples_ReturnsRightTriangleCount(
        int[][] grid, long expected) =>
        Assert.Equal(expected, RightTrianglesSolution.CountByTalliedRowsAndColumns(grid));
}
