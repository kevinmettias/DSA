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
    private ServerNetwork(IReadOnlyList<ServerNode> servers) => Servers = servers;

    // Every server, including any the connection list never mentions: the bridge
    // search is a multi-root walk and only visits the components its roots reach.
    public IReadOnlyList<ServerNode> Servers { get; }

    public static ServerNetwork Build(int serverCount, int[][] connections)
    {
        var servers = new ServerNode[serverCount];

        for (var id = 0; id < serverCount; id++)
        {
            servers[id] = new ServerNode(id);
        }

        foreach (var connection in connections)
        {
            var first = servers[connection[0]];
            var second = servers[connection[1]];
            first.Neighbors.Add(second);
            second.Neighbors.Add(first);
        }

        return new ServerNetwork(servers);
    }
}
