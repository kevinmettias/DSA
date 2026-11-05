using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.Solutions.FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree;

// LC 1489's promise about its input: the graph is connected, so it has a minimum spanning tree at all.
// LeetCode's judge never poses an input that breaks it, and both strategies throw on one that does.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreePromise
{
    public static bool IsKeptBy(int nodeCount, int[][] edges) => IsConnected(nodeCount, edges);

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
