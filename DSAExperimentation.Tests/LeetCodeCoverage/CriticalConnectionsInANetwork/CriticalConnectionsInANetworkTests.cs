using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.CriticalConnectionsInANetwork.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CriticalConnectionsInANetwork;

// LeetCode 1192. Critical Connections in a Network: a critical connection is
// exactly a bridge - an edge whose removal disconnects the network - so this is a
// direct call into this repo's own low-link bridge-finding DFS
// (BridgesAndArticulationPoints.Find), reading only the Bridges half of its tuple
// result over an undirected topology (each connection stored as two directed
// child-edges, one from each endpoint, the same convention
// BridgesAndArticulationPointsTests/MinimumSpanningTree.cs already use).
public sealed partial class CriticalConnectionsInANetworkTests
{
    [Fact]
    public void CriticalConnections_LeetCodeExampleOne_ReturnsTheSinglePendantEdge()
    {
        var servers = BuildNetwork(4, [(0, 1), (1, 2), (2, 0), (1, 3)]);

        var result = FindCriticalConnections(servers);

        Assert.Equal([[1, 3]], NormalizeConnections(result));
    }

    [Fact]
    public void CriticalConnections_RingOfServers_HasNoCriticalConnection()
    {
        var servers = BuildNetwork(4, [(0, 1), (1, 2), (2, 3), (3, 0)]);

        var result = FindCriticalConnections(servers);

        Assert.Empty(result);
    }

    [Fact]
    public void CriticalConnections_TwoTrianglesJoinedByOneLink_ReturnsThatLink()
    {
        var servers = BuildNetwork(6, [(0, 1), (1, 2), (2, 0), (3, 4), (4, 5), (5, 3), (2, 3)]);

        var result = FindCriticalConnections(servers);

        Assert.Equal([[2, 3]], NormalizeConnections(result));
    }

    private static List<ServerNode> BuildNetwork(int serverCount, (int A, int B)[] connections)
    {
        var servers = Enumerable.Range(0, serverCount).Select(id => new ServerNode(id)).ToList();

        foreach (var (a, b) in connections)
        {
            servers[a].Neighbors.Add(servers[b]);
            servers[b].Neighbors.Add(servers[a]);
        }

        return servers;
    }

    private static List<(ServerNode A, ServerNode B)> FindCriticalConnections(List<ServerNode> servers)
    {
        var (bridges, _) = BridgesAndArticulationPoints.Find<
            ServerNode, ServerTopology, ListChildren<ServerNode>,
            NaturalChildOrder<ServerNode, ListChildren<ServerNode>>, ListChildren<ServerNode>>(
            servers);

        return bridges;
    }

    // Bridge membership, not discovery order, is the contract LeetCode's own
    // "return in any order" spec cares about, so both the pair and the outer list
    // sort to a stable order before comparison.
    private static List<List<int>> NormalizeConnections(List<(ServerNode A, ServerNode B)> connections)
        => connections
            .Select(connection => new List<int> { connection.A.Id, connection.B.Id }.OrderBy(id => id).ToList())
            .OrderBy(pair => pair[0])
            .ThenBy(pair => pair[1])
            .ToList();
}
