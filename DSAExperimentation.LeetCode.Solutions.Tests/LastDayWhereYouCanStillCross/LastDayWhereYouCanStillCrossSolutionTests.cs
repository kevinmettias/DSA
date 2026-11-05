using DSAExperimentation.LeetCode.Solutions.LastDayWhereYouCanStillCross;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LastDayWhereYouCanStillCross;

// Harness only: both bisection strategies live in
// LastDayWhereYouCanStillCrossSolution and are asserted against the same cases - a
// single cell, a one-row grid and a non-square grid, since every published example is
// square and the two arms compute their bounds from the row and column counts
// separately. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class LastDayWhereYouCanStillCrossSolutionTests
{
    public static TheoryData<int, int, int[][], int> Cases =>
        new()
        {
            { 1, 1, [[1, 1]], 0 }, // the only cell is both top and bottom row, dry on day 0 alone
            { 1, 2, [[1, 1], [1, 2]], 1 }, // one row: crossing survives while any cell of it is land
            { 2, 3, [[1, 1], [2, 2], [1, 3], [2, 1], [1, 2], [2, 3]], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LatestDayToCrossByManualBisection_Cases_ReturnsLastCrossableDay(
        int row, int col, int[][] cells, int expected)
    {
        var actual = LastDayWhereYouCanStillCrossSolution.LatestDayToCrossByManualBisection(row, col, cells);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LatestDayToCrossByPredicateSearch_Cases_ReturnsLastCrossableDay(
        int row, int col, int[][] cells, int expected)
    {
        var actual = LastDayWhereYouCanStillCrossSolution.LatestDayToCrossByPredicateSearch(row, col, cells);

        Assert.Equal(expected, actual);
    }
}
