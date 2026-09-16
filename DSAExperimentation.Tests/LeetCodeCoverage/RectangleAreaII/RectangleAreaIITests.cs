using DSAExperimentation.LeetCode.RectangleAreaII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RectangleAreaII;

// Harness only. Both strategies are RectangleAreaIISolution's - the O(n^3)
// compressed-cell check and the IntervalSet sweep line - pinned here to
// LeetCode's published examples plus disjoint and nested shapes. The single
// 10^9 x 10^9 rectangle is the case that forces the overflow-safe modular
// accumulation: 10^18 mod (10^9 + 7) is 49.
public sealed partial class RectangleAreaIITests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0, 0, 2, 2], [1, 0, 2, 3], [1, 0, 3, 1]], 6 },
            { [[0, 0, 1_000_000_000, 1_000_000_000]], 49 },
            { [[0, 0, 2, 2], [10, 10, 12, 12]], 8 },
            { [[0, 0, 3, 3], [1, 1, 2, 2]], 9 },
            { [[0, 0, 2, 2], [0, 0, 2, 2]], 4 },
            { [[0, 0, 1, 1]], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalAreaByCoordinateCompression_LeetCodeExamples_ReturnsUnionAreaModuloOneBillionSeven(
        int[][] rectangles, int expected) =>
        Assert.Equal(expected, RectangleAreaIISolution.TotalAreaByCoordinateCompression(rectangles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalAreaBySweepLine_LeetCodeExamples_ReturnsUnionAreaModuloOneBillionSeven(
        int[][] rectangles, int expected) =>
        Assert.Equal(expected, RectangleAreaIISolution.TotalAreaBySweepLine(rectangles));
}
