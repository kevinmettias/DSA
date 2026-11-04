using DSAExperimentation.LeetCode.Solutions.CountLatticePointsInsideACircle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountLatticePointsInsideACircle;

public sealed partial class CountLatticePointsInsideACirclePromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[2, 2, 1]], true },
            { [[2, 2, 2], [3, 4, 1]], true },
            { [[1, 2, 3]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryCircleStaysInTheFirstQuadrant(int[][] circles, bool expected)
        => Assert.Equal(expected, CountLatticePointsInsideACirclePromise.IsKeptBy(circles));
}
