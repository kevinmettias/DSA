using DSAExperimentation.LeetCode.SurfaceAreaOf3DShapes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SurfaceAreaOf3DShapes;

// Harness only: both strategies live in SurfaceAreaOf3DShapesSolution. The padded-
// border arm was previously benchmark-only and unasserted; the hollow grid
// ([[1,1,1],[1,0,1],[1,1,1]]) and the single tall column are the cases that would
// expose an off-by-one in its border offsets or a dropped height-0 skip.
public sealed class SurfaceAreaOf3DShapesTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[2]], 10 },
            { [[1, 2], [3, 4]], 34 },
            { [[1, 1, 1], [1, 0, 1], [1, 1, 1]], 32 },
            { [[2, 2, 2], [2, 1, 2], [2, 2, 2]], 46 },
            { [[0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SurfaceAreaByBoundsCheckedNeighbors_LeetCodeExamples_SumsExposedFaces(
        int[][] grid, int expected) =>
        Assert.Equal(expected, SurfaceAreaOf3DShapesSolution.SurfaceAreaByBoundsCheckedNeighbors(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SurfaceAreaByPaddedBorder_LeetCodeExamples_SumsExposedFaces(
        int[][] grid, int expected) =>
        Assert.Equal(expected, SurfaceAreaOf3DShapesSolution.SurfaceAreaByPaddedBorder(grid));
}
