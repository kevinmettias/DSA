using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.Solutions.CollectCoinsInATree;

// LC 2603's promise about its input: the edges make a tree of the nodes 0 to n - 1, one node per coin.
// LeetCode's judge never poses an input that breaks it. Both strategies build an adjacency list of n
// slots, which an edge to a node past n runs off. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class CollectCoinsInATreePromise
{
    public static bool IsKeptBy(int[] coins, int[][] edges)
        => edges.Length == coins.Length - 1 && edges.All(edge => edge.All(node => node >= 0 && node < coins.Length)) && IsAcyclic(coins.Length, edges);

    // Each edge joins two nodes no earlier edge has connected; with n - 1 edges that connects them all.
    private static bool IsAcyclic(int nodeCount, int[][] edges)
    {
        var components = new DisjointSet(nodeCount);

        foreach (var edge in edges)
        {
            if (components.IsConnected(edge[0], edge[1]))
            {
                return false;
            }

            components.Union(edge[0], edge[1]);
        }

        return true;
    }
}
