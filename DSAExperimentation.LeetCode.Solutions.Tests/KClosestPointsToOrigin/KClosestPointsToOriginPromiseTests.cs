using DSAExperimentation.LeetCode.Solutions.KClosestPointsToOrigin;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KClosestPointsToOrigin;

public sealed partial class KClosestPointsToOriginPromiseTests
{
    public static TheoryData<int[][], int, bool> Inputs =>
        new()
        {
            { [[1, 3], [-2, 2]], 1, true },
            { [[3, 3], [5, -1], [-2, 4]], 2, true },
            { [[1, 0], [0, 1]], 1, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoTieFallsAtTheKthDistance(int[][] points, int nearestCount, bool expected)
        => Assert.Equal(expected, KClosestPointsToOriginPromise.IsKeptBy(points, nearestCount));
}
