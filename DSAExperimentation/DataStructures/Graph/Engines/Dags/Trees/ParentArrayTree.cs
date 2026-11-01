namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Materializes LeetCode's parent-array encoding of a rooted tree: parent[i] is the
// id of i's parent, with a negative entry marking the root.
/// <summary>Builds <see cref="RootedTreeNode"/> trees from LeetCode's parent-array encoding.</summary>
public static class ParentArrayTree
{
    /// <summary>Creates one node per entry of a parent array and links each node under its parent.</summary>
    /// <param name="parent">Each node's parent id, indexed by node id, with a negative entry for the root.</param>
    /// <returns>Every node, indexed by id, with each node's children listed in increasing id order.</returns>
    public static RootedTreeNode[] Build(int[] parent)
    {
        var nodes = new RootedTreeNode[parent.Length];

        for (var i = 0; i < parent.Length; i++)
        {
            nodes[i] = new RootedTreeNode(i);
        }

        for (var i = 0; i < parent.Length; i++)
        {
            if (parent[i] >= 0)
            {
                nodes[parent[i]].Children.Add(nodes[i]);
            }
        }

        return nodes;
    }

    // A straight chain - node i's parent is i - 1 - the deepest tree a given node
    // count admits, and the input shape that makes any per-node cost proportional
    // to subtree size show up as genuine O(n^2) work.
    /// <summary>Builds a chain where node i's parent is node i - 1, the deepest tree of that size.</summary>
    /// <param name="nodeCount">How many nodes the chain holds, at least 1.</param>
    /// <returns>The chain's root, node 0.</returns>
    public static RootedTreeNode Chain(int nodeCount)
    {
        var nodes = Build(ChainParents(nodeCount));

        return nodes[0];
    }

    private static int[] ChainParents(int nodeCount)
    {
        var parent = new int[nodeCount];
        parent[0] = -1;

        for (var i = 1; i < nodeCount; i++)
        {
            parent[i] = i - 1;
        }

        return parent;
    }
}
