using DSAExperimentation.LeetCode.Solutions.MinimumAreaRectangleII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumAreaRectangleII;

// Harness only. Both strategies are MinimumAreaRectangleIISolution's; this file
// states three cases once (an axis-aligned rectangle among stray points, a single
// point, and overlapping axis-aligned rectangles) and asserts every strategy against
// them - the O(n^4) quadruple-scan baseline included, which the benchmark
// previously measured without anything checking its answer. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class MinimumAreaRectangleIISolutionTests
{
    private const int AreaPrecision = 5;

    public static TheoryData<int[][], double> Cases =>
        new()
        {
            // An axis-aligned 4x3 rectangle with two unrelated points mixed in.
            { [[0, 0], [4, 0], [4, 3], [0, 3], [2, 2], [5, 5]], 12.0 },

            // A single point has no candidate diagonal at all.
            { [[0, 0]], 0.0 },

            // Several overlapping axis-aligned rectangles; the 1x2 one is smallest.
            { [[3, 1], [1, 1], [0, 1], [2, 1], [3, 3], [3, 2], [0, 2], [2, 3]], 2.0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinAreaFreeRectByQuadrupleScan_Cases_ReturnsSmallestRectangleArea(
        int[][] points, double expected) =>
        Assert.Equal(expected, MinimumAreaRectangleIISolution.MinAreaFreeRectByQuadrupleScan(points), AreaPrecision);

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinAreaFreeRectByDiagonalGrouping_Cases_ReturnsSmallestRectangleArea(
        int[][] points, double expected) =>
        Assert.Equal(expected, MinimumAreaRectangleIISolution.MinAreaFreeRectByDiagonalGrouping(points), AreaPrecision);
}
