using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.TrimABinarySearchTree;

// LeetCode 669. Trim a Binary Search Tree: drop every node outside [low, high]
// and return the root of what remains, keeping the relative structure of the
// nodes that stay - a kept node's descendants remain its descendants.
//
// TrimByInPlaceMutation is the textbook recursion: an out-of-range node is
// dropped by returning its in-range child's own trimmed result in its place - the
// same "return the replacement subtree root, caller reassigns node.Left/Right"
// shape BinarySearchTree.TryDelete already uses. It visits every node.
//
// TrimByIterativeBoundaryWalk reads the BST order instead: everything strictly
// inside the range is already correct, so only two paths can need work - the
// left boundary, where a left child below `low` is replaced by its own right
// subtree, and the right boundary, where a right child above `high` is replaced by
// its left subtree. It visits O(height) nodes rather than all of them.
//
// A strategy that collects the in-range values and reinserts them into a fresh
// tree used to sit here as the baseline. It kept the right values but not the
// structure LC 669 requires - reinserting sorted values builds a chain - and its
// tests compared only in-order values, which every BST with those values shares.
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

    public static BinaryTreeNode<int>? TrimByIterativeBoundaryWalk(BinaryTreeNode<int>? root, int low, int high)
    {
        while (root is not null && (root.Value < low || root.Value > high))
        {
            root = root.Value < low ? root.Right : root.Left;
        }

        for (var node = root; node is not null; node = node.Left)
        {
            while (node.Left is { } left && left.Value < low)
            {
                node.Left = left.Right;
            }
        }

        for (var node = root; node is not null; node = node.Right)
        {
            while (node.Right is { } right && right.Value > high)
            {
                node.Right = right.Left;
            }
        }

        return root;
    }
}
