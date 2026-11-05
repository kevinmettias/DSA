using DSAExperimentation.LeetCode.Solutions.MaxPointsOnALine;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaxPointsOnALine;

// Harness only. Both strategies are MaxPointsOnALineSolution's - this file just
// pins them to two duplicate-points cases the slope-grouping strategy has to
// answer without double-counting or losing exact duplicates of the anchor. The
// last case is the one the pre-migration benchmark's slope-grouping arm got wrong:
// it bucketed a point coinciding with the anchor under its own trivial (0,0)
// "slope" instead of adding it to whichever real line wins, so it never found the
// line through all four points. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MaxPointsOnALineSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[0, 0], [0, 0], [1, 1], [2, 2]], 4 },
            { [[0, 0], [0, 0], [5, 5], [5, 5]], 4 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxPointsByCrossProduct_Cases_ReturnsLargestLineCount(
        int[][] points, int expected) =>
        Assert.Equal(expected, MaxPointsOnALineSolution.MaxPointsByCrossProduct(points));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxPointsBySlopeGrouping_Cases_ReturnsLargestLineCount(
        int[][] points, int expected) =>
        Assert.Equal(expected, MaxPointsOnALineSolution.MaxPointsBySlopeGrouping(points));
}
