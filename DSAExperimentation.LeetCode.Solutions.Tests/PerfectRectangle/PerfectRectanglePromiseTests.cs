using DSAExperimentation.LeetCode.Solutions.PerfectRectangle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PerfectRectangle;

public sealed partial class PerfectRectanglePromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 1, 3, 3], [3, 1, 4, 2], [3, 2, 4, 4], [1, 3, 2, 4], [2, 3, 3, 4]], true },
            { [[-1, -1, -1, 3]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryRectangleHasArea(int[][] rectangles, bool expected)
        => Assert.Equal(expected, PerfectRectanglePromise.IsKeptBy(rectangles));
}
