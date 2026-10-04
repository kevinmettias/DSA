using DSAExperimentation.LeetCode.MaximizeTheDistanceBetweenPointsOnASquare;

namespace DSAExperimentation.LeetCode.Tests.MaximizeTheDistanceBetweenPointsOnASquare;

// Harness only. The perimeter mapping and both binary-search-on-the-answer
// strategies are MaximizeTheDistanceBetweenPointsOnASquareSolution's - this file
// pins both strategies to LeetCode's published examples and the perimeter mapping to
// offsets worked out by hand.
public sealed partial class MaximizeTheDistanceBetweenPointsOnASquareSolutionTests
{
    public static TheoryData<int, int[][], int, int> Examples =>
        new()
        {
            { 2, new[] { new[] { 0, 2 }, new[] { 2, 0 }, new[] { 2, 2 }, new[] { 0, 0 } }, 4, 2 },
            { 2, new[] { new[] { 0, 0 }, new[] { 1, 2 }, new[] { 2, 0 }, new[] { 2, 2 }, new[] { 2, 1 } }, 4, 1 },
            {
                2,
                new[]
                {
                    new[] { 0, 0 }, new[] { 0, 1 }, new[] { 0, 2 }, new[] { 1, 2 },
                    new[] { 2, 0 }, new[] { 2, 2 }, new[] { 2, 1 },
                },
                5,
                1
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDistanceByLinearScan_LeetCodeExamples_ReturnsMaximizedMinimumDistance(
        int side, int[][] points, int selectionCount, int expected)
    {
        var actual = MaximizeTheDistanceBetweenPointsOnASquareSolution.MaxDistanceByLinearScan(
            side, points, selectionCount);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDistanceBySortedGreedy_LeetCodeExamples_ReturnsMaximizedMinimumDistance(
        int side, int[][] points, int selectionCount, int expected)
    {
        var actual = MaximizeTheDistanceBetweenPointsOnASquareSolution.MaxDistanceBySortedGreedy(
            side, points, selectionCount);
        Assert.Equal(expected, actual);
    }

    // The perimeter walked clockwise from (0, 0) on a side of 2: up the left edge at y,
    // along the top at 2 + x, down the right edge at 6 - y, back along the bottom at
    // 8 - x. (0,0) -> 0, (1,2) -> 3, (2,0) -> 6, (2,2) -> 4, (2,1) -> 5, sorted.
    [Fact]
    public void ToSortedPerimeterPositions_LeetCodeSecondExample_ReturnsClockwiseOffsetsAscending()
    {
        var positions = MaximizeTheDistanceBetweenPointsOnASquareSolution.ToSortedPerimeterPositions(
            2, [[0, 0], [1, 2], [2, 0], [2, 2], [2, 1]]);

        Assert.Equal([0L, 3L, 4L, 5L, 6L], Enumerable.Range(0, positions.Length).Select(positions.Get));
    }

    // One point inside each edge of a side-4 square, given out of order, so every one of
    // the four mappings is used once: (3,0) on the bottom -> 16 - 3 = 13, (4,3) on the
    // right -> 12 - 3 = 9, (0,1) on the left -> 1, (2,4) on the top -> 4 + 2 = 6.
    [Fact]
    public void ToSortedPerimeterPositions_OnePointPerEdge_MapsEachEdgeToItsOwnStretch()
    {
        var positions = MaximizeTheDistanceBetweenPointsOnASquareSolution.ToSortedPerimeterPositions(
            4, [[3, 0], [4, 3], [0, 1], [2, 4]]);

        Assert.Equal([1L, 6L, 9L, 13L], Enumerable.Range(0, positions.Length).Select(positions.Get));
    }
}
