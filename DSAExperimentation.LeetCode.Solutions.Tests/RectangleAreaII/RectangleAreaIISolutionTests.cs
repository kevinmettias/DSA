using DSAExperimentation.LeetCode.Solutions.RectangleAreaII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RectangleAreaII;

// Harness only. Both strategies are RectangleAreaIISolution's - the O(n^3)
// compressed-cell check and the IntervalSet sweep line - pinned here to
// disjoint and nested shapes. The single
// 10^9 x 10^9 rectangle is the case that forces the overflow-safe modular
// accumulation: 10^18 mod (10^9 + 7) is 49.
public sealed partial class RectangleAreaIISolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[0, 0, 2, 2], [10, 10, 12, 12]], 8 },
            { [[0, 0, 3, 3], [1, 1, 2, 2]], 9 },
            { [[0, 0, 2, 2], [0, 0, 2, 2]], 4 },
            { [[0, 0, 1, 1]], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void TotalAreaByCoordinateCompression_Cases_ReturnsUnionAreaModuloOneBillionSeven(
        int[][] rectangles, int expected) =>
        Assert.Equal(expected, RectangleAreaIISolution.TotalAreaByCoordinateCompression(rectangles));

    [Theory]
    [MemberData(nameof(Cases))]
    public void TotalAreaBySweepLine_Cases_ReturnsUnionAreaModuloOneBillionSeven(
        int[][] rectangles, int expected) =>
        Assert.Equal(expected, RectangleAreaIISolution.TotalAreaBySweepLine(rectangles));
}
