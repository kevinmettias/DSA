using DSAExperimentation.LeetCode.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

// Harness only. The road network is CityGraph and both shortest-path strategies
// are FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution's -
// this file just pins them to LeetCode's published examples plus the edge cases
// the original test left uncovered (a lone city, and a graph where some pair is
// unreachable at any distance).
public sealed class FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceTests
{
    public static TheoryData<int, int[][], int, int> Examples =>
        new()
        {
            { 4, [[0, 1, 3], [1, 2, 1], [1, 3, 4], [2, 3, 1]], 4, 3 },
            { 5, [[0, 1, 2], [0, 4, 8], [1, 2, 3], [1, 4, 2], [2, 3, 1], [3, 4, 1]], 2, 0 },
            { 1, [], 1, 0 },
            { 3, [[0, 1, 1]], 1, 2 },
            { 3, [[0, 1, 1], [1, 2, 1]], 1, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCityByDijkstraPerSource_LeetCodeExamples_ReturnsCityWithFewestReachableNeighbors(
        int n, int[][] edges, int distanceThreshold, int expected) =>
        Assert.Equal(
            expected,
            FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution
                .FindCityByDijkstraPerSource(n, edges, distanceThreshold));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCityByFloydWarshall_LeetCodeExamples_ReturnsCityWithFewestReachableNeighbors(
        int n, int[][] edges, int distanceThreshold, int expected) =>
        Assert.Equal(
            expected,
            FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution
                .FindCityByFloydWarshall(n, edges, distanceThreshold));
}
