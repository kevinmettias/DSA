using DSAExperimentation.LeetCode.MinimumCostToReachDestinationInTime;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToReachDestinationInTime;

// Harness only. The road map and its (city, elapsedTime) expansion are
// MinimumCostToReachDestinationInTimeSolution's own RoadNetwork/TimeCityGraph and
// both strategies are its methods - this file pins them to LeetCode's three
// published examples, which share one road map and differ only in the budget, plus
// four the original test did not cover: a single road that just fits, the same road
// with no budget at all, and a pair where the cheap route is the slow one and only
// the looser budget can afford it. The naive walk is asserted here for the first
// time; it used to live in the benchmark as an unasserted baseline.
public sealed class MinimumCostToReachDestinationInTimeTests
{
    public static TheoryData<int, int[][], int[], int> Examples =>
        new()
        {
            // LC example 1: 0 -> 1 -> 2 -> 5 costs 5 + 1 + 2 + 3 = 11 and takes
            // exactly 30 minutes.
            {
                30,
                [[0, 1, 10], [1, 2, 10], [2, 5, 10], [0, 3, 1], [3, 4, 10], [4, 5, 15]],
                [5, 1, 2, 20, 20, 3],
                11
            },

            // LC example 2: one minute short of the cheap route, so the 26-minute
            // 0 -> 3 -> 4 -> 5 detour has to be paid for instead.
            {
                29,
                [[0, 1, 10], [1, 2, 10], [2, 5, 10], [0, 3, 1], [3, 4, 10], [4, 5, 15]],
                [5, 1, 2, 20, 20, 3],
                48
            },

            // LC example 3: no route at all fits 25 minutes.
            {
                25,
                [[0, 1, 10], [1, 2, 10], [2, 5, 10], [0, 3, 1], [3, 4, 10], [4, 5, 15]],
                [5, 1, 2, 20, 20, 3],
                -1
            },

            // A single road that exactly spends the budget: both fees are charged.
            { 1, [[0, 1, 1]], [1, 2], 3 },

            // The same road with no minutes to spend at all.
            { 0, [[0, 1, 1]], [1, 2], -1 },

            // The slow road is the cheap one: 0 -> 2 takes ten minutes and costs 2,
            // while the two-minute detour pays 50 to pass through city 1.
            { 10, [[0, 2, 10], [0, 1, 1], [1, 2, 1]], [1, 50, 1], 2 },

            // One minute short of the cheap road, so the expensive detour wins.
            { 9, [[0, 2, 10], [0, 1, 1], [1, 2, 1]], [1, 50, 1], 52 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByNaiveDfs_LeetCodeExamples_ReturnsCheapestFeeSumWithinBudget(
        int maxTime, int[][] edges, int[] passingFees, int expected)
    {
        var actual = MinimumCostToReachDestinationInTimeSolution.MinCostByNaiveDfs(maxTime, edges, passingFees);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByStateExpandedDijkstra_LeetCodeExamples_ReturnsCheapestFeeSumWithinBudget(
        int maxTime, int[][] edges, int[] passingFees, int expected)
    {
        var actual = MinimumCostToReachDestinationInTimeSolution.MinCostByStateExpandedDijkstra(
            maxTime, edges, passingFees);

        Assert.Equal(expected, actual);
    }
}
