using DSAExperimentation.LeetCode.Solutions.RectangleOverlap;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RectangleOverlap;

public sealed partial class RectangleOverlapPromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [0, 0, 2, 2], [1, 1, 3, 3], true },
            { [0, 0, 1, 1], [1, 0, 2, 1], true },
            { [1, 1, 10, 10], [5, 5, 5, 10], false },
            { [0, 0, 2, 0], [0, 0, 1, 1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForTwoRectanglesOfNonZeroArea(int[] rec1, int[] rec2, bool expected)
        => Assert.Equal(expected, RectangleOverlapPromise.IsKeptBy(rec1, rec2));
}
