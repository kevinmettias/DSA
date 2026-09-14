using DSAExperimentation.LeetCode.LastDayWhereYouCanStillCross;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LastDayWhereYouCanStillCross;

// Harness only: both bisection strategies live in
// LastDayWhereYouCanStillCrossSolution and are asserted against the same examples -
// LeetCode's own three, plus a single cell, a one-row grid and a non-square grid,
// since every published example is square and the two arms compute their bounds from
// the row and column counts separately.
public sealed class LastDayWhereYouCanStillCrossTests
{
    public static TheoryData<int, int, int[][], int> Examples =>
        new()
        {
            { 2, 2, [[1, 1], [2, 1], [1, 2], [2, 2]], 2 },
            { 2, 2, [[1, 1], [1, 2], [2, 1], [2, 2]], 1 },
            {
                3, 3,
                [[1, 2], [2, 1], [3, 3], [2, 2], [1, 1], [1, 3], [2, 3], [3, 2], [3, 1]],
                3
            },
            { 1, 1, [[1, 1]], 0 }, // the only cell is both top and bottom row, dry on day 0 alone
            { 1, 2, [[1, 1], [1, 2]], 1 }, // one row: crossing survives while any cell of it is land
            { 2, 3, [[1, 1], [2, 2], [1, 3], [2, 1], [1, 2], [2, 3]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LatestDayToCrossByManualBisection_LeetCodeExamples_ReturnsLastCrossableDay(
        int row, int col, int[][] cells, int expected) =>
        Assert.Equal(expected, LastDayWhereYouCanStillCrossSolution.LatestDayToCrossByManualBisection(row, col, cells));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LatestDayToCrossBySequenceLowerBound_LeetCodeExamples_ReturnsLastCrossableDay(
        int row, int col, int[][] cells, int expected) =>
        Assert.Equal(
            expected, LastDayWhereYouCanStillCrossSolution.LatestDayToCrossBySequenceLowerBound(row, col, cells));
}
