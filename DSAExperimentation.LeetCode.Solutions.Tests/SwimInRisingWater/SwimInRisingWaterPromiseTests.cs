using DSAExperimentation.LeetCode.Solutions.SwimInRisingWater;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SwimInRisingWater;

public sealed partial class SwimInRisingWaterPromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[0, 2], [1, 3]], true },
            { [[0, 1, 2, 3, 4], [24, 23, 22, 21, 5], [12, 13, 14, 15, 16], [11, 17, 18, 19, 20], [10, 9, 8, 7, 6]], true },
            { [[3, 0], [0, 2]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForEachElevationOnce(int[][] grid, bool expected)
        => Assert.Equal(expected, SwimInRisingWaterPromise.IsKeptBy(grid));
}
