using DSAExperimentation.LeetCode.Solutions.CountNumberOfRectanglesContainingEachPoint;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountNumberOfRectanglesContainingEachPoint;

public sealed partial class CountNumberOfRectanglesContainingEachPointPromiseTests
{
    public static TheoryData<int[][], int[][], bool> Inputs =>
        new()
        {
            { [[1, 2], [2, 3], [2, 5]], [[2, 1], [1, 4]], true },
            { [[100, 100], [200, 200]], [[100, 100]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoHeightPassesOneHundred(int[][] rectangles, int[][] points, bool expected)
        => Assert.Equal(expected, CountNumberOfRectanglesContainingEachPointPromise.IsKeptBy(rectangles, points));
}
