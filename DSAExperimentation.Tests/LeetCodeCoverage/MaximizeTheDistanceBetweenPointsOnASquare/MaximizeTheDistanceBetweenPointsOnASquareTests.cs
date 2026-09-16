using DSAExperimentation.LeetCode.MaximizeTheDistanceBetweenPointsOnASquare;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeTheDistanceBetweenPointsOnASquare;

// Harness only. The perimeter mapping and both binary-search-on-the-answer
// strategies are MaximizeTheDistanceBetweenPointsOnASquareSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class MaximizeTheDistanceBetweenPointsOnASquareTests
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
}
