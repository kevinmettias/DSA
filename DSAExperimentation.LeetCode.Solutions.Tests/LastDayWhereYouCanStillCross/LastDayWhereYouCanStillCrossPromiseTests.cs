using DSAExperimentation.LeetCode.Solutions.LastDayWhereYouCanStillCross;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LastDayWhereYouCanStillCross;

public sealed partial class LastDayWhereYouCanStillCrossPromiseTests
{
    public static TheoryData<int, int, int[][], bool> Inputs =>
        new()
        {
            { 2, 2, [[1, 1], [2, 1], [1, 2], [2, 2]], true },
            { 2, 2, [[1, 1], [2, 1], [1, 2]], false },
            { 2, 2, [[1, 1], [1, 1], [1, 2], [2, 2]], false },
            { 2, 2, [[1, 1], [2, 1], [1, 2], [3, 2]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryCellFloodsOnce(int row, int col, int[][] cells, bool expected)
        => Assert.Equal(expected, LastDayWhereYouCanStillCrossPromise.IsKeptBy(row, col, cells));
}
