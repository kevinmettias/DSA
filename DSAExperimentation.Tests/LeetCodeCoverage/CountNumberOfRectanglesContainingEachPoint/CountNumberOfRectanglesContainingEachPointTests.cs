using DSAExperimentation.LeetCode.CountNumberOfRectanglesContainingEachPoint;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountNumberOfRectanglesContainingEachPoint;

// Harness only. Both strategies - the O(n*m) brute-force scan and the grouped
// HashMap + MergeSort + double-LowerBound composition - live in
// CountNumberOfRectanglesContainingEachPointSolution; this file pins each of them
// to LeetCode's published examples plus the boundary cases the bisecting strategy
// has to get right (a point past every length, exact equality on both coordinates,
// and a height above every rectangle).
public sealed class CountNumberOfRectanglesContainingEachPointTests
{
    public static TheoryData<int[][], int[][], int[]> Examples =>
        new()
        {
            { [[1, 2], [2, 3], [2, 5]], [[2, 1], [1, 4]], [2, 1] },
            { [[1, 1], [2, 2], [3, 3]], [[1, 3], [1, 1]], [1, 3] },
            { [[1, 1]], [[2, 1]], [0] },
            { [[5, 5]], [[5, 5], [5, 6], [6, 5]], [1, 0, 0] },
            { [[2, 3], [3, 2], [4, 4]], [[2, 2], [3, 3], [4, 4], [1, 5]], [3, 1, 1, 0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRectanglesByBruteForce_LeetCodeExamples_ReturnsCoveringCountPerPoint(
        int[][] rectangles, int[][] points, int[] expected)
    {
        var actual = CountNumberOfRectanglesContainingEachPointSolution.CountRectanglesByBruteForce(rectangles, points);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRectanglesByGroupedLowerBound_LeetCodeExamples_ReturnsCoveringCountPerPoint(
        int[][] rectangles, int[][] points, int[] expected)
    {
        var actual = CountNumberOfRectanglesContainingEachPointSolution.CountRectanglesByGroupedLowerBound(rectangles, points);
        Assert.Equal(expected, actual);
    }
}
