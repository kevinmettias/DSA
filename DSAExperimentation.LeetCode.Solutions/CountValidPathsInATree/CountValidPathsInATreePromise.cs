using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.Solutions.CountValidPathsInATree;

// LC 2867's promise about its input: the edges make a tree of the nodes 1 to n. LeetCode's judge never
// poses an input that breaks it. The path walk indexes its parent table by node, past its end for a
// node beyond n. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class CountValidPathsInATreePromise
{
    public static bool IsKeptBy(int nodeCount, int[][] edges)
        => edges.Length == nodeCount - 1 && edges.All(edge => edge.All(node => node >= 1 && node <= nodeCount)) && IsAcyclic(nodeCount, edges);

    // No edge joins two nodes that the edges before it have already connected. With n - 1 edges, that
    // also connects every node.
    private static bool IsAcyclic(int nodeCount, int[][] edges)
    {
        // Ids from 0 to n, so a node's id is its number; id 0 stays alone.
        var components = new DisjointSet(nodeCount + 1);

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
