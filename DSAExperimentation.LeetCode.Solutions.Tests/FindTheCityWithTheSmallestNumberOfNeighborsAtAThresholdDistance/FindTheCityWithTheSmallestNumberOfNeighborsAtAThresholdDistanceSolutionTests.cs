using DSAExperimentation.LeetCode.Solutions.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistance;

// Harness only. The road network is CityGraph and every shortest-path strategy is
// FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution's -
// this file just pins them to the edge cases
// the original test left uncovered (a lone city, and a graph where some pair is
// unreachable at any distance).
public sealed partial class FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolutionTests
{
    public static TheoryData<int, int[][], int, int> Cases =>
        new()
        {
            { 1, [], 1, 0 },
            { 3, [[0, 1, 1]], 1, 2 },
            { 3, [[0, 1, 1], [1, 2, 1]], 1, 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindCityByDistanceMatrix_Cases_ReturnsCityWithFewestReachableNeighbors(
        int cityCount, int[][] edges, int distanceThreshold, int expected)
    {
        var actual = FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution
            .FindCityByDistanceMatrix(cityCount, edges, distanceThreshold);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindCityByDijkstraPerSource_Cases_ReturnsCityWithFewestReachableNeighbors(
        int cityCount, int[][] edges, int distanceThreshold, int expected)
    {
        var actual = FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution
            .FindCityByDijkstraPerSource(cityCount, edges, distanceThreshold);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindCityByFloydWarshall_Cases_ReturnsCityWithFewestReachableNeighbors(
        int cityCount, int[][] edges, int distanceThreshold, int expected)
    {
        var actual = FindTheCityWithTheSmallestNumberOfNeighborsAtAThresholdDistanceSolution
            .FindCityByFloydWarshall(cityCount, edges, distanceThreshold);

        Assert.Equal(expected, actual);
    }
}
