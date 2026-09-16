using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.ValidateBinarySearchTree;

// LeetCode 98. Validate Binary Search Tree: every node's value must fall strictly
// between the open bound its ancestors have narrowed it to.
//
// A hand-rolled recursion over BinaryTreeNode<int>, not TreeFold/BinaryTreeChildren,
// because the check needs each child position's presence-or-absence directly - the
// ListChildren witness compacts missing children away, losing exactly the shape
// information a BST bounds check depends on.
internal static class ValidateBinarySearchTreeSolution
{
    public static bool IsValidByBoundsRecursion(BinaryTreeNode<int>? root) =>
        IsWithinBounds(root, null, null);

    private static bool IsWithinBounds(BinaryTreeNode<int>? node, int? min, int? max) =>
        node is null ||
        ((min is null || node.Value > min) &&
         (max is null || node.Value < max) &&
         IsWithinBounds(node.Left, min, node.Value) &&
         IsWithinBounds(node.Right, node.Value, max));
}
