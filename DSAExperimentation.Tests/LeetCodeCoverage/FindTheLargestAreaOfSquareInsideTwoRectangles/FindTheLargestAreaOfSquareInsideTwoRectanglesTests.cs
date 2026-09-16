using DSAExperimentation.LeetCode.FindTheLargestAreaOfSquareInsideTwoRectangles;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheLargestAreaOfSquareInsideTwoRectangles;

// Harness only: both pairwise strategies live in
// FindTheLargestAreaOfSquareInsideTwoRectanglesSolution - this file just
// pins them to LeetCode's published examples.
public sealed class FindTheLargestAreaOfSquareInsideTwoRectanglesTests
{
    public static TheoryData<int[][], int[][], long> Examples =>
        new()
        {
            { [[1, 1], [2, 2], [3, 1]], [[3, 3], [4, 4], [6, 6]], 1 },
            { [[1, 1], [1, 3], [1, 5]], [[5, 5], [5, 7], [5, 9]], 4 },
            { [[1, 1], [2, 2], [1, 2]], [[3, 3], [4, 4], [3, 4]], 1 },
            { [[1, 1], [3, 3], [3, 1]], [[2, 2], [4, 4], [4, 2]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestSquareAreaByBruteForcePairs_LeetCodeExamples_ReturnsLargestFittingSquareArea(
        int[][] bottomLeft, int[][] topRight, long expected)
    {
        var actual = FindTheLargestAreaOfSquareInsideTwoRectanglesSolution.LargestSquareAreaByBruteForcePairs(
            bottomLeft, topRight);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestSquareAreaBySortedPrunedPairs_LeetCodeExamples_ReturnsLargestFittingSquareArea(
        int[][] bottomLeft, int[][] topRight, long expected)
    {
        var actual = FindTheLargestAreaOfSquareInsideTwoRectanglesSolution.LargestSquareAreaBySortedPrunedPairs(
            bottomLeft, topRight);

        Assert.Equal(expected, actual);
    }
}
