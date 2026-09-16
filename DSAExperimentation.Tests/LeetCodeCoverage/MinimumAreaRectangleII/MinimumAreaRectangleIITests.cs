using DSAExperimentation.LeetCode.MinimumAreaRectangleII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAreaRectangleII;

// Harness only. Both strategies are MinimumAreaRectangleIISolution's; this file
// states LeetCode's published examples once and asserts every strategy against
// them - the O(n^4) quadruple-scan baseline included, which the benchmark
// previously measured without anything checking its answer.
public sealed partial class MinimumAreaRectangleIITests
{
    private const int AreaPrecision = 5;

    public static TheoryData<int[][], double> Examples =>
        new()
        {
            // LC example 1: a square rotated 45 degrees -> 2.00000
            { [[1, 2], [2, 1], [1, 0], [0, 1]], 2.0 },

            // LC example 2: the unit square (1,0)-(2,0)-(2,1)-(1,1) -> 1.00000
            { [[0, 1], [2, 1], [1, 1], [1, 0], [2, 0]], 1.0 },

            // LC example 3: no four points bisect each other at equal length -> 0
            { [[0, 3], [1, 2], [3, 1], [1, 3], [2, 1]], 0.0 },

            // An axis-aligned 4x3 rectangle with two unrelated points mixed in.
            { [[0, 0], [4, 0], [4, 3], [0, 3], [2, 2], [5, 5]], 12.0 },

            // Fewer than four points cannot form any rectangle.
            { [[1, 1], [2, 2], [3, 3]], 0.0 },

            // A single point has no candidate diagonal at all.
            { [[0, 0]], 0.0 },

            // Several overlapping axis-aligned rectangles; the 1x2 one is smallest.
            { [[3, 1], [1, 1], [0, 1], [2, 1], [3, 3], [3, 2], [0, 2], [2, 3]], 2.0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAreaFreeRectByQuadrupleScan_LeetCodeExamples_ReturnsSmallestRectangleArea(
        int[][] points, double expected) =>
        Assert.Equal(expected, MinimumAreaRectangleIISolution.MinAreaFreeRectByQuadrupleScan(points), AreaPrecision);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAreaFreeRectByDiagonalGrouping_LeetCodeExamples_ReturnsSmallestRectangleArea(
        int[][] points, double expected) =>
        Assert.Equal(expected, MinimumAreaRectangleIISolution.MinAreaFreeRectByDiagonalGrouping(points), AreaPrecision);
}
