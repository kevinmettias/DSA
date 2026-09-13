using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.RangeSumOfBST;

// LeetCode 938. Range Sum of BST: sum every value in [low, high] held by a binary
// search tree.
//
// RangeSumByFullScan is the naive baseline - the answer you would write for a plain
// binary tree, visiting all n nodes and testing each against the range, ignoring the
// BST ordering invariant entirely.
//
// RangeSumBySearchTreePruning uses that invariant to skip whole subtrees: a node
// below low can only hold something relevant in its right subtree, a node above
// high only in its left - the same "return the recursive result from one side" shape
// TrimABinarySearchTreeSolution.TrimByInPlaceMutation uses for the same problem
// family, summing survivors instead of re-parenting them.
internal static class RangeSumOfBSTSolution
{
    // The textbook full-tree walk: every node visited, ordering never consulted.
    // Deliberately plain recursion over BinaryTreeNode<int> - it is the arm the
    // pruned walk below has to justify itself against.
    public static int RangeSumByFullScan(BinaryTreeNode<int>? node, int low, int high)
    {
        if (node is null)
        {
            return 0;
        }

        var contribution = node.Value >= low && node.Value <= high ? node.Value : 0;

        return contribution
            + RangeSumByFullScan(node.Left, low, high)
            + RangeSumByFullScan(node.Right, low, high);
    }

    // Prune on the BST ordering invariant: an out-of-range node proves one of its
    // subtrees cannot contain anything in [low, high], so that side is never visited.
    public static int RangeSumBySearchTreePruning(BinaryTreeNode<int>? node, int low, int high)
    {
        if (node is null)
        {
            return 0;
        }

        if (node.Value < low)
        {
            return RangeSumBySearchTreePruning(node.Right, low, high);
        }

        if (node.Value > high)
        {
            return RangeSumBySearchTreePruning(node.Left, low, high);
        }

        return node.Value
            + RangeSumBySearchTreePruning(node.Left, low, high)
            + RangeSumBySearchTreePruning(node.Right, low, high);
    }
}
