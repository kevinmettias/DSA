using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 501: a random BST of a given size, built with
// duplicates allowed (value <= node.Value goes left), which this repo's own
// BinarySearchTree<TValue>.Insert rejects as a no-op - LeetCode 501's BST
// explicitly permits repeated values, so this builds BinaryTreeNode<int> nodes
// directly instead, with roughly nodesPerDistinctValue nodes sharing each value
// so the tree has realistic duplicate runs.
internal static class FindModeInBinarySearchTreeWorkloads
{
    public static BinaryTreeNode<int>? BuildTree(int nodeCount, int nodesPerDistinctValue, int seed)
    {
        var distinctValues = Math.Max(1, nodeCount / nodesPerDistinctValue);
        var values = new int[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            values[i] = i % distinctValues;
        }

        var random = new Random(seed);
        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        BinaryTreeNode<int>? root = null;
        foreach (var value in values)
        {
            InsertAllowingDuplicates(ref root, value);
        }

        return root;
    }

    private static void InsertAllowingDuplicates(ref BinaryTreeNode<int>? root, int value)
    {
        if (root is null)
        {
            root = new BinaryTreeNode<int>(value);
            return;
        }

        var node = root;

        // Stops when the walk reaches the null child slot the value belongs in, where the new node is linked and the method returns.
        while (true)
        {
            if (value <= node.Value)
            {
                if (node.Left is null)
                {
                    node.Left = new BinaryTreeNode<int>(value);
                    return;
                }

                node = node.Left;
            }
            else
            {
                if (node.Right is null)
                {
                    node.Right = new BinaryTreeNode<int>(value);
                    return;
                }

                node = node.Right;
            }
        }
    }
}
