using DSAExperimentation.LeetCode.NetworkDelayTime;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NetworkDelayTime;

// Harness only. The graph representation is NetworkDelayTimeSolution's own
// NetworkNode/NetworkTopology and all three shortest-path strategies are its
// methods - this file just pins them to LeetCode's published examples, including
// the two shapes of "some node is unreachable" (no edges at all, and an edge that
// only leads away from the source).
public sealed class NetworkDelayTimeTests
{
    public static TheoryData<int[][], int, int, int> Examples =>
        new()
        {
            { [[2, 1, 1], [2, 3, 1], [3, 4, 1]], 4, 2, 2 },
            { [], 2, 1, -1 },
            { [[1, 2, 1]], 2, 1, 1 },
            { [[1, 2, 1]], 2, 2, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinutesToReachAllByDijkstra_LeetCodeExamples_ReturnsMaxDistanceFromSource(
        int[][] times, int n, int k, int expected)
    {
        var minutes = NetworkDelayTimeSolution.MinutesToReachAllByDijkstra(times, n, k);

        Assert.Equal(expected, minutes);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinutesToReachAllByBellmanFord_LeetCodeExamples_ReturnsMaxDistanceFromSource(
        int[][] times, int n, int k, int expected)
    {
        var minutes = NetworkDelayTimeSolution.MinutesToReachAllByBellmanFord(times, n, k);

        Assert.Equal(expected, minutes);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinutesToReachAllByFloydWarshall_LeetCodeExamples_ReturnsMaxDistanceFromSource(
        int[][] times, int n, int k, int expected)
    {
        var minutes = NetworkDelayTimeSolution.MinutesToReachAllByFloydWarshall(times, n, k);

        Assert.Equal(expected, minutes);
    }
}
