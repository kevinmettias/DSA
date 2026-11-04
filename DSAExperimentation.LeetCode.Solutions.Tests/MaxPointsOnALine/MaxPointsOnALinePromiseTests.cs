using DSAExperimentation.LeetCode.Solutions.MaxPointsOnALine;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaxPointsOnALine;

public sealed partial class MaxPointsOnALinePromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 1], [2, 2], [3, 3]], true },
            { [[1, 1], [1, 1], [2, 2]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenThePointsAreUnique(int[][] points, bool expected)
        => Assert.Equal(expected, MaxPointsOnALinePromise.IsKeptBy(points));
}
