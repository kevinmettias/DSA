using DSAExperimentation.LeetCode.Solutions.MinimumAreaRectangle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumAreaRectangle;

public sealed partial class MinimumAreaRectanglePromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 1], [1, 3], [3, 1], [3, 3], [2, 2]], true },
            { [[10, 10], [10, 15], [10, 10]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenThePointsAreUnique(int[][] points, bool expected)
        => Assert.Equal(expected, MinimumAreaRectanglePromise.IsKeptBy(points));
}
