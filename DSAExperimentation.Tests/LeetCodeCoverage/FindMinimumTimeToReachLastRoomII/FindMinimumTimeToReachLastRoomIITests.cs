using DSAExperimentation.LeetCode.FindMinimumTimeToReachLastRoomII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMinimumTimeToReachLastRoomII;

// Harness only: the algorithms live in FindMinimumTimeToReachLastRoomIISolution.
// One test method per strategy over one shared set of LeetCode's own examples, so
// a failure names the strategy that broke (TwoSumTests precedent).
public sealed class FindMinimumTimeToReachLastRoomIITests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0, 4], [4, 4]], 7 },
            { [[0, 0, 0, 0], [0, 0, 0, 0]], 6 },
            { [[0, 1], [1, 2]], 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByBclPriorityQueue_LeetCodeExamples_ReturnsMinimumArrivalTime(int[][] moveTime, int expected)
    {
        var actual = FindMinimumTimeToReachLastRoomIISolution.MinTimeByBclPriorityQueue(moveTime);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByHeap_LeetCodeExamples_ReturnsMinimumArrivalTime(int[][] moveTime, int expected)
    {
        var actual = FindMinimumTimeToReachLastRoomIISolution.MinTimeByHeap(moveTime);
        Assert.Equal(expected, actual);
    }
}
