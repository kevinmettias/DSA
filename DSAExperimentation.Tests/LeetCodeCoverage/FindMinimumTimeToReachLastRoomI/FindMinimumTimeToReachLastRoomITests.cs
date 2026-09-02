using DSAExperimentation.LeetCode.FindMinimumTimeToReachLastRoomI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMinimumTimeToReachLastRoomI;

// Harness only: the algorithms live in FindMinimumTimeToReachLastRoomISolution. One
// test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke (TwoSumTests precedent).
public sealed class FindMinimumTimeToReachLastRoomITests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0, 4], [4, 4]], 6 },
            { [[0, 0, 0], [0, 0, 0]], 3 },
            { [[0, 1], [1, 2]], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByBclPriorityQueue_LeetCodeExamples_ReturnsMinimumArrivalTime(int[][] moveTime, int expected) =>
        Assert.Equal(expected, FindMinimumTimeToReachLastRoomISolution.MinimumTimeByBclPriorityQueue(moveTime));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByHeap_LeetCodeExamples_ReturnsMinimumArrivalTime(int[][] moveTime, int expected) =>
        Assert.Equal(expected, FindMinimumTimeToReachLastRoomISolution.MinimumTimeByHeap(moveTime));
}
