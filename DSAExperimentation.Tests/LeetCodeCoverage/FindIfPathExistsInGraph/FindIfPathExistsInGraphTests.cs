using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindIfPathExistsInGraph;

// LeetCode 1971. Find if Path Exists in Graph: union every edge into this repo's
// own DisjointSet (NumberOfOperationsToMakeNetworkConnectedTests/
// NumberOfProvincesTests precedent), then source and destination have a path
// between them exactly when DisjointSet.IsConnected says their roots match - path
// existence in an undirected graph reduces to plain connectivity, no traversal
// needed.
public sealed partial class FindIfPathExistsInGraphTests
{
    [Fact]
    public void ValidPath_DirectAndIndirectEdgesConnectSourceAndDestination_ReturnsTrue()
    {
        int[][] edges = [[0, 1], [1, 2], [2, 0]];

        var reachable = ValidPath(n: 3, edges, source: 0, destination: 2);

        Assert.True(reachable);
    }

    [Fact]
    public void ValidPath_DestinationInDisconnectedComponent_ReturnsFalse()
    {
        int[][] edges = [[0, 1], [0, 2], [3, 5], [5, 4], [4, 3]];

        var reachable = ValidPath(n: 6, edges, source: 0, destination: 5);

        Assert.False(reachable);
    }

    [Fact]
    public void ValidPath_SourceEqualsDestination_ReturnsTrue()
    {
        int[][] edges = [];

        var reachable = ValidPath(n: 1, edges, source: 0, destination: 0);

        Assert.True(reachable);
    }

    private static bool ValidPath(int n, int[][] edges, int source, int destination)
    {
        var components = new DisjointSet(n);

        foreach (var edge in edges)
        {
            components.Union(edge[0], edge[1]);
        }

        return components.IsConnected(source, destination);
    }
}
