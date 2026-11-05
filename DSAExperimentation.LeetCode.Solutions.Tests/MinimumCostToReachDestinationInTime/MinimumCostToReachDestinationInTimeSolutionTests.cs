using DSAExperimentation.LeetCode.Solutions.MinimumCostToReachDestinationInTime;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumCostToReachDestinationInTime;

// Harness only. The road map and its (city, elapsedTime) expansion are
// MinimumCostToReachDestinationInTimeSolution's own RoadNetwork/TimeCityGraph and
// both strategies are its methods - this file pins them to four cases on two small
// road maps: a single road that just fits, the same road with no budget at all, and
// a pair where the cheap route is the slow one and only the looser budget can afford
// it. The naive walk is asserted here for the first time; it used to live in the
// benchmark as an unasserted baseline. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MinimumCostToReachDestinationInTimeSolutionTests
{
    public static TheoryData<int, int[][], int[], int> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void MinCostByNaiveDfs_Cases_ReturnsCheapestFeeSumWithinBudget(
        int maxTime, int[][] edges, int[] passingFees, int expected)
    {
        var actual = MinimumCostToReachDestinationInTimeSolution.MinCostByNaiveDfs(maxTime, edges, passingFees);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinCostByStateExpandedDijkstra_Cases_ReturnsCheapestFeeSumWithinBudget(
        int maxTime, int[][] edges, int[] passingFees, int expected)
    {
        var actual = MinimumCostToReachDestinationInTimeSolution.MinCostByStateExpandedDijkstra(
            maxTime, edges, passingFees);

        Assert.Equal(expected, actual);
    }
}
