using DSAExperimentation.LeetCode.Solutions.ShortestDistanceAfterRoadAdditionQueriesII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ShortestDistanceAfterRoadAdditionQueriesII;

public sealed partial class ShortestDistanceAfterRoadAdditionQueriesIIPromiseTests
{
    public static TheoryData<int, int[][], bool> Inputs =>
        new()
        {
            { 5, [[2, 4], [0, 2], [0, 4]], true },
            { 4, [[0, 3], [0, 2]], true },
            { 8, [[2, 7], [0, 2], [4, 6], [0, 4]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoTwoRoadsCross(int cityCount, int[][] queries, bool expected)
        => Assert.Equal(expected, ShortestDistanceAfterRoadAdditionQueriesIIPromise.IsKeptBy(cityCount, queries));
}
