using DSAExperimentation.LeetCode.Solutions.MaximizeTheDistanceBetweenPointsOnASquare;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximizeTheDistanceBetweenPointsOnASquare;

public sealed partial class MaximizeTheDistanceBetweenPointsOnASquarePromiseTests
{
    public static TheoryData<int, int[][], int, bool> Inputs =>
        new()
        {
            { 2, [[0, 2], [2, 0], [2, 2], [0, 0]], 4, true },
            { 2, [[0, 0], [1, 1], [2, 0], [0, 2]], 4, false },
            { 2, [[0, 0], [0, 0], [2, 0], [0, 2]], 4, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryPointIsOnTheBoundaryOnce(int side, int[][] points, int selectionCount, bool expected)
        => Assert.Equal(expected, MaximizeTheDistanceBetweenPointsOnASquarePromise.IsKeptBy(side, points, selectionCount));
}
