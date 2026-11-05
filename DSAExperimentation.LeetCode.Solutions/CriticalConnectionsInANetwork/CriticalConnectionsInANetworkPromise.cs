using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.Solutions.CriticalConnectionsInANetwork;

// LC 1192's promise about its input: any server can reach any other, so the network is connected.
// LeetCode's judge never poses an input that breaks it. A connection off on its own is a bridge to the
// low-link search and none to the removal scan, which counts components against a connected whole.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class CriticalConnectionsInANetworkPromise
{
    public static bool IsKeptBy(int serverCount, int[][] connections) => IsConnected(serverCount, connections);

    // Every node joined to node 0 once each edge between nodes of the graph has joined its two ends.
    private static bool IsConnected(int nodeCount, int[][] edges)
    {
        var components = new DisjointSet(nodeCount);

        foreach (var edge in edges.Where(edge => edge.All(node => node >= 0 && node < nodeCount)))
        {
            components.Union(edge[0], edge[1]);
        }

        return Enumerable.Range(1, nodeCount - 1).All(node => components.IsConnected(0, node));
    }
}
