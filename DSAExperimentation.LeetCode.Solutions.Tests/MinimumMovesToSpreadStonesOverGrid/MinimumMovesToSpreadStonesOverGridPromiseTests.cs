using DSAExperimentation.LeetCode.Solutions.MinimumMovesToSpreadStonesOverGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumMovesToSpreadStonesOverGrid;

public sealed partial class MinimumMovesToSpreadStonesOverGridPromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 1, 0], [1, 1, 1], [1, 2, 1]], true },
            { [[2, 2, 1], [1, 1, 1], [1, 1, 1]], false },
            { [[9]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForNineStonesOnThreeByThree(int[][] grid, bool expected)
        => Assert.Equal(expected, MinimumMovesToSpreadStonesOverGridPromise.IsKeptBy(grid));
}
