
// One server of LC 1192's undirected network.
using ServerNode = DSAExperimentation.DataStructures.Graph.Adjacency.AdjacencyNode;

namespace DSAExperimentation.LeetCode.CriticalConnectionsInANetwork;

// LC 1192's connection list, materialized as nodes. Each connection is stored as
// two directed child-edges, one from each endpoint - the undirected convention
// Algorithms.Connectivity.BridgesAndArticulationPoints documents and
// MinimumSpanningTree already uses.
//
// This exists as a type rather than a bare node list so a benchmark can hoist
// construction into [GlobalSetup] and hand the prepared network to the strategy's
// second overload without that overload becoming ambiguous with the
// LeetCode-shaped one (ARCHITECTURE.md #17.4).
internal sealed class ServerNetwork
{
    // Every server, including any the connection list never mentions: the bridge
    // search is a multi-root walk and only visits the components its roots reach.
    public IReadOnlyList<ServerNode> Servers { get; }

    private ServerNetwork(IReadOnlyList<ServerNode> servers) => Servers = servers;

    // A ServerNode per server id, then both directions of every connection - the
    // layout LeetCodeAdjacency states once for every problem taking an (n, edges) pair.
    public static ServerNetwork Build(int serverCount, int[][] connections)
    {
        var servers = LeetCodeAdjacency.ZeroBased<ServerNode>(
            serverCount, connections, id => new ServerNode(id), (server, _, farServer, _) => server.Neighbors.Add(farServer));

        return new ServerNetwork(servers);
    }
}
