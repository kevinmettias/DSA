using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.TrimABinarySearchTree;

// LeetCode 669. Trim a Binary Search Tree: drop every node outside [low, high]
// and return the root of what remains.
//
// TrimByInPlaceMutation is the O(n) single-pass approach: an out-of-range node
// is dropped by returning its in-range child's own trimmed result in its place -
// the same "return the replacement subtree root, caller reassigns
// node.Left/node.Right" shape BinarySearchTree.TryDelete already uses.
//
// TrimByCollectAndRebuild is the naive baseline it is measured against: a full
// in-order walk collects every in-range value, then a fresh BinarySearchTree<int>
// is rebuilt by reinserting them one at a time - paying an O(n) descent per
// insert (O(n^2) worst case on an already-sorted/skewed sequence) instead of
// reattaching nodes that are already there.
internal static class TrimABinarySearchTreeSolution
{
    public static BinaryTreeNode<int>? TrimByInPlaceMutation(BinaryTreeNode<int>? node, int low, int high)
    {
        if (node is null)
        {
            return null;
        }

        if (node.Value < low)
        {
            return TrimByInPlaceMutation(node.Right, low, high);
        }

        if (node.Value > high)
        {
            return TrimByInPlaceMutation(node.Left, low, high);
        }

        node.Left = TrimByInPlaceMutation(node.Left, low, high);
        node.Right = TrimByInPlaceMutation(node.Right, low, high);
        return node;
    }

    public static BinaryTreeNode<int>? TrimByCollectAndRebuild(BinaryTreeNode<int>? node, int low, int high)
    {
        var kept = new List<int>();
        CollectInRange(node, low, high, kept);

        var tree = new BinarySearchTree<int>();

        foreach (var value in kept)
        {
            tree.Insert(value);
        }

        return tree.Root;
    }

    private static void CollectInRange(BinaryTreeNode<int>? node, int low, int high, List<int> kept)
    {
        if (node is null)
        {
            return;
        }

        CollectInRange(node.Left, low, high, kept);

        if (node.Value >= low && node.Value <= high)
        {
            kept.Add(node.Value);
        }

        CollectInRange(node.Right, low, high, kept);
    }
}
