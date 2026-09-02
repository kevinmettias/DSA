namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Materializes LeetCode's parent-array encoding of a rooted tree: parent[i] is the
// id of i's parent, with a negative entry marking the root.
internal static class ParentArrayTree
{
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
