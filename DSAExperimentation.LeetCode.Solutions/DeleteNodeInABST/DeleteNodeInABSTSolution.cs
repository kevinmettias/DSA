using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.DeleteNodeInABST;

// LeetCode 450. Delete Node in a BST: real LeetCode hands you a TreeNode root and a
// key and wants the root back with that key gone. This repo's own
// BinarySearchTree<int> is the target API for that shape - Insert builds the input
// tree, and both strategies below take one and hand back the tree with the key
// removed, so a caller checks the result the same way for either: Has/
// InOrderTraversal, the composition KthSmallestElementInABSTSolutionTests already uses.
[LeetCodeQuestion("delete-node-in-a-bst", LeetCodeAnswerOrder.AsWritten, Promise = typeof(DeleteNodeInABSTPromise), AnswerCheck = typeof(DeleteNodeInABSTAnswerCheck))]
internal static class DeleteNodeInABSTSolution
{
    // LeetCode's shape (ARCHITECTURE 17.4): the tree's root and the key, and the root back. The search tree
    // both strategies take is built by inserting the tree's values in level order, which builds the same tree.
    public static BinaryTreeNode<int>? DeleteByCollectFilterRebuild(BinaryTreeNode<int>? root, int key)
    {
        var tree = SearchTreeOf(root);

        return DeleteByCollectFilterRebuild(tree, key).Root;
    }

    public static BinaryTreeNode<int>? DeleteByBinarySearchTreeDelete(BinaryTreeNode<int>? root, int key)
    {
        var tree = SearchTreeOf(root);

        return DeleteByBinarySearchTreeDelete(tree, key).Root;
    }

    // The naive approach many first solutions reach for: walk the whole tree
    // in-order, drop the key from the resulting sorted values, and insert what
    // remains into a fresh tree - root-first so height stays O(log n) instead of
    // degenerating on already-sorted input. Touches every node on every call,
    // unlike the O(h) in-place delete below. Deliberately never reuses
    // BinarySearchTree.TryDelete - only Insert, to place the rebuilt values -
    // since TryDelete is exactly what this baseline exists to be measured against.
    public static BinarySearchTree<int> DeleteByCollectFilterRebuild(BinarySearchTree<int> tree, int key)
    {
        var remaining = new List<int>();
        CollectInOrder(tree.Root, remaining);
        remaining.Remove(key);

        var rebuilt = new BinarySearchTree<int>();
        InsertBalanced(rebuilt, remaining, 0, remaining.Count - 1);
        return rebuilt;
    }

    // This repo's own BinarySearchTree<int>.TryDelete: O(h), reattaching only the
    // affected subtree via in-order-successor promotion for the two-child case
    // (see BinarySearchTree.cs's own DeleteFoundNode), never touching an unrelated
    // node. Mutates and returns the same instance - TryDelete already is the O(h)
    // delete, so there is nothing left for this method to add.
    public static BinarySearchTree<int> DeleteByBinarySearchTreeDelete(BinarySearchTree<int> tree, int key)
    {
        tree.TryDelete(key);
        return tree;
    }

    private static BinarySearchTree<int> SearchTreeOf(BinaryTreeNode<int>? root)
    {
        var tree = new BinarySearchTree<int>();
        var levelOrder = LeetCodeWireFormat.FromBinaryTree(root).OfType<int>();

        foreach (var value in levelOrder)
        {
            tree.Insert(value);
        }

        return tree;
    }

    private static void CollectInOrder(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        CollectInOrder(node.Left, values);
        values.Add(node.Value);
        CollectInOrder(node.Right, values);
    }

    // Inserting the midpoint first, then recursing on each half, reproduces the
    // same balanced shape a from-scratch array-based build would - insertion
    // order alone determines a BST's shape for a fixed set of values.
    private static void InsertBalanced(BinarySearchTree<int> tree, List<int> values, int low, int high)
    {
        if (low > high)
        {
            return;
        }

        var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);
        tree.Insert(values[mid]);
        InsertBalanced(tree, values, low, mid - 1);
        InsertBalanced(tree, values, mid + 1, high);
    }
}
