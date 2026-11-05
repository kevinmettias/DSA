using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.Solutions.RedundantConnectionII;

// LC 685's promise about its input: the edges are a rooted tree of the nodes 1 to n with one more
// directed edge added. LeetCode's judge never poses an input that breaks it. With no edge whose removal
// leaves a rooted tree, the removal scan finds nothing to answer and the disjoint-set pass answers an
// edge anyway. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class RedundantConnectionIIPromise
{
    public static bool IsKeptBy(int[][] edges)
        => edges.All(edge => edge.All(node => node >= 1 && node <= edges.Length))
            && Enumerable.Range(0, edges.Length).Any(removed => IsRootedTree(edges.Length, [.. edges.Where((_, index) => index != removed)]));

    // Every node but one has exactly one parent, and the edges, read without direction, connect all n
    // nodes: with n - 1 edges that is a tree, rooted at the node with no parent.
    private static bool IsRootedTree(int nodeCount, int[][] edges)
    {
        var parentCounts = new int[nodeCount + 1];
        var components = new DisjointSet(nodeCount + 1);

        foreach (var edge in edges)
        {
            parentCounts[edge[1]]++;
            components.Union(edge[0], edge[1]);
        }

        var nodes = Enumerable.Range(1, nodeCount).ToList();
        return nodes.Count(node => parentCounts[node] == 0) == 1
            && nodes.All(node => parentCounts[node] <= 1 && components.IsConnected(1, node));
    }
}
