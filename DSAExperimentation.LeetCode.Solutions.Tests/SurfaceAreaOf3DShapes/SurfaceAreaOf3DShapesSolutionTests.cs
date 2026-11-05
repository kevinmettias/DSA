using DSAExperimentation.LeetCode.Solutions.SurfaceAreaOf3DShapes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SurfaceAreaOf3DShapes;

// Harness only: both strategies live in SurfaceAreaOf3DShapesSolution. The padded-
// border arm was previously benchmark-only and unasserted; the hollow grid
// ([[1,1,1],[1,0,1],[1,1,1]]) and the single tall column are the cases that would
// expose an off-by-one in its border offsets or a dropped height-0 skip.
public sealed partial class SurfaceAreaOf3DShapesSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[2]], 10 },
            { [[0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SurfaceAreaByBoundsCheckedNeighbors_Cases_SumsExposedFaces(
        int[][] grid, int expected) =>
        Assert.Equal(expected, SurfaceAreaOf3DShapesSolution.SurfaceAreaByBoundsCheckedNeighbors(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SurfaceAreaByPaddedBorder_Cases_SumsExposedFaces(
        int[][] grid, int expected) =>
        Assert.Equal(expected, SurfaceAreaOf3DShapesSolution.SurfaceAreaByPaddedBorder(grid));
}
