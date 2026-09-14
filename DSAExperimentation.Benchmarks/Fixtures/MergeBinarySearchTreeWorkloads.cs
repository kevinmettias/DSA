using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for Merge BSTs to Create Single BST (LC 1932): how many
// trees the forest holds, and a fresh copy of it per invocation. The merge itself is
// MergeBSTsToCreateSingleBSTSolution's.
internal static class MergeBinarySearchTreeWorkloads
{
    // Tree i: root = treeCount - i, left leaf = treeCount - i - 1, which is the next
    // tree's root value - a strictly descending left-leaning chain once merged, so
    // BST order holds, every tree but the last gets spliced away, and the only thing
    // treeCount changes is how much work "find the tree whose root matches this leaf"
    // has to do.
    public static List<BinaryTreeNode<int>> BuildChain(int treeCount)
    {
        var trees = new List<BinaryTreeNode<int>>(treeCount);

        for (var i = 0; i < treeCount; i++)
        {
            var rootValue = treeCount - i;
            var tree = new BinaryTreeNode<int>(rootValue);

            if (rootValue > 1)
            {
                tree.Left = new BinaryTreeNode<int>(rootValue - 1);
            }

            trees.Add(tree);
        }

        return trees;
    }

    // Splicing mutates Left/Right in place, so the forest is consumed by the merge
    // and cannot be prepared once in [GlobalSetup]: every invocation needs its own
    // copy, and both arms pay for it identically.
    public static List<BinaryTreeNode<int>> Clone(IReadOnlyList<BinaryTreeNode<int>> forest)
    {
        var clones = new List<BinaryTreeNode<int>>(forest.Count);

        foreach (var tree in forest)
        {
            clones.Add(CloneNode(tree));
        }

        return clones;
    }

    private static BinaryTreeNode<int> CloneNode(BinaryTreeNode<int> node)
    {
        var clone = new BinaryTreeNode<int>(node.Value);

        if (node.Left is { } left)
        {
            clone.Left = CloneNode(left);
        }

        if (node.Right is { } right)
        {
            clone.Right = CloneNode(right);
        }

        return clone;
    }
}
