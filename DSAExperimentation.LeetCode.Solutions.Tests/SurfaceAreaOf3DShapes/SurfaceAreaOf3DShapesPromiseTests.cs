using DSAExperimentation.LeetCode.Solutions.SurfaceAreaOf3DShapes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SurfaceAreaOf3DShapes;

public sealed partial class SurfaceAreaOf3DShapesPromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 2], [3, 4]], true },
            { [[1, 2, 3], [4, 5, 6]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForASquareGrid(int[][] grid, bool expected)
        => Assert.Equal(expected, SurfaceAreaOf3DShapesPromise.IsKeptBy(grid));
}
