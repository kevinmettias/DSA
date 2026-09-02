using DSAExperimentation.LeetCode.MaxPointsOnALine;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxPointsOnALine;

// Harness only. Both strategies are MaxPointsOnALineSolution's - this file just
// pins them to LeetCode's published examples, plus two duplicate-points cases
// the slope-grouping strategy has to answer without double-counting or losing
// exact duplicates of the anchor. The last example is the one the pre-migration
// benchmark's slope-grouping arm got wrong: it bucketed a point coinciding with
// the anchor under its own trivial (0,0) "slope" instead of adding it to
// whichever real line wins, so it never found the line through all four points.
public sealed class MaxPointsOnALineTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 1], [2, 2], [3, 3]], 3 },
            { [[1, 1], [3, 2], [5, 3], [4, 1], [2, 3], [1, 4]], 4 },
            { [[0, 0], [0, 0], [1, 1], [2, 2]], 4 },
            { [[0, 0], [0, 0], [5, 5], [5, 5]], 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPointsByCrossProduct_LeetCodeExamples_ReturnsLargestLineCount(
        int[][] points, int expected) =>
        Assert.Equal(expected, MaxPointsOnALineSolution.MaxPointsByCrossProduct(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPointsBySlopeGrouping_LeetCodeExamples_ReturnsLargestLineCount(
        int[][] points, int expected) =>
        Assert.Equal(expected, MaxPointsOnALineSolution.MaxPointsBySlopeGrouping(points));
}
