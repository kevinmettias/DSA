using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToArriveAtDestination;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToArriveAtDestination;

public sealed partial class NumberOfWaysToArriveAtDestinationPromiseTests
{
    public static TheoryData<int, int[][], bool> Inputs =>
        new()
        {
            { 7, [[0, 6, 7], [0, 1, 2], [1, 2, 3], [1, 3, 3], [6, 3, 3], [3, 5, 1], [6, 5, 1], [2, 5, 1], [0, 4, 5], [4, 6, 2]], true },
            { 3, [[0, 1, 1], [1, 2, 2], [1, 0, 5]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoRoadRepeatsOrLoops(int intersectionCount, int[][] roads, bool expected)
        => Assert.Equal(expected, NumberOfWaysToArriveAtDestinationPromise.IsKeptBy(intersectionCount, roads));
}
