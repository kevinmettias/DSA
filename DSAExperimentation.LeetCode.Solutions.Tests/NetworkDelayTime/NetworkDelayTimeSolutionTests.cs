using DSAExperimentation.LeetCode.Solutions.NetworkDelayTime;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NetworkDelayTime;

// Harness only. The graph representation is NetworkDelayTimeSolution's own
// NetworkNode/NetworkTopology and every shortest-path strategy is one of its
// methods - this file just pins them to a network with no edges at all, the
// simplest shape of "some node is unreachable". LeetCode's published examples run
// in CatalogCaseTests.
public sealed partial class NetworkDelayTimeSolutionTests
{
    public static TheoryData<int[][], int, int, int> Cases =>
        new()
        {
            { [], 2, 1, -1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinutesToReachAllByPriorityQueue_Cases_ReturnsMaxDistanceFromSource(
        int[][] times, int nodeCount, int sourceNodeId, int expected)
    {
        var minutes = NetworkDelayTimeSolution.MinutesToReachAllByPriorityQueue(times, nodeCount, sourceNodeId);

        Assert.Equal(expected, minutes);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinutesToReachAllByDijkstra_Cases_ReturnsMaxDistanceFromSource(
        int[][] times, int nodeCount, int sourceNodeId, int expected)
    {
        var minutes = NetworkDelayTimeSolution.MinutesToReachAllByDijkstra(times, nodeCount, sourceNodeId);

        Assert.Equal(expected, minutes);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinutesToReachAllByBellmanFord_Cases_ReturnsMaxDistanceFromSource(
        int[][] times, int nodeCount, int sourceNodeId, int expected)
    {
        var minutes = NetworkDelayTimeSolution.MinutesToReachAllByBellmanFord(times, nodeCount, sourceNodeId);

        Assert.Equal(expected, minutes);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinutesToReachAllByFloydWarshall_Cases_ReturnsMaxDistanceFromSource(
        int[][] times, int nodeCount, int sourceNodeId, int expected)
    {
        var minutes = NetworkDelayTimeSolution.MinutesToReachAllByFloydWarshall(times, nodeCount, sourceNodeId);

        Assert.Equal(expected, minutes);
    }
}
