using DSAExperimentation.LeetCode.Solutions.FindTheNumberOfWaysToPlacePeopleI;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheNumberOfWaysToPlacePeopleI;

public sealed partial class FindTheNumberOfWaysToPlacePeopleIPromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[1, 1], [2, 2], [3, 3]], true },
            { [[6, 2], [4, 4], [2, 6]], true },
            { [[5, 5], [1, 1], [5, 5]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenThePointsAreDistinct(int[][] points, bool expected)
        => Assert.Equal(expected, FindTheNumberOfWaysToPlacePeopleIPromise.IsKeptBy(points));
}
