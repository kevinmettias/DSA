using DSAExperimentation.LeetCode.CountNumberOfTrapezoidsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountNumberOfTrapezoidsII;

// Harness only. The general-slope trapezoid counting itself is
// CountNumberOfTrapezoidsIISolution's - this file just pins both strategies
// to LeetCode's published examples, including the axis-aligned case shared
// with Part I's own second example (a horizontal trapezoid is a special case
// of a general one).
public sealed class CountNumberOfTrapezoidsIITests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[-3, 2], [3, 0], [2, 3], [3, 2], [2, -3]], 2 },
            { [[0, 0], [1, 0], [0, 1], [2, 1]], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountTrapezoidsByBruteForce_LeetCodeExamples_ReturnsTrapezoidCount(
        int[][] points, int expected) =>
        Assert.Equal(expected, CountNumberOfTrapezoidsIISolution.CountTrapezoidsByBruteForce(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountTrapezoidsByParallelSegmentCounting_LeetCodeExamples_ReturnsTrapezoidCount(
        int[][] points, int expected) =>
        Assert.Equal(expected, CountNumberOfTrapezoidsIISolution.CountTrapezoidsByParallelSegmentCounting(points));
}
