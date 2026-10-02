using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.BalancedBinaryTree;

// LeetCode 110. Balanced Binary Tree: is every node's two subtree heights
// within one of each other?
//
// Two strategies. The bottom-up recursion below folds height computation and the
// balance check into a single pass, returning -1 the moment a subtree is found
// unbalanced and short-circuiting the rest of the walk. The top-down check above it is
// the naive counterpart: at every node it measures each subtree's height from scratch,
// so heights are recomputed once per ancestor and the cost degrades to O(n log n) on a
// balanced tree. The old test's private helper and both of the old benchmark's
// [Benchmark] arms called the bottom-up recursion under different names, so only the
// top-down arm is genuinely new.
internal static class BalancedBinaryTreeSolution
{
    private const int UnbalancedHeightMarker = -1;
    private const int MaxAllowedHeightDifference = 1;

    // The naive top-down counterpart to the single-pass recursion below: at this node it
    // measures both subtrees' heights from scratch, rejects an imbalance here, and
    // otherwise recurses to check the children. Every subtree is therefore re-measured
    // once per ancestor, which is the O(n log n) cost the bottom-up arm avoids with its
    // sentinel - but it assumes nothing about when an imbalance is found, and it reaches
    // the same verdict.
    public static bool IsBalancedByTopDownHeightCheck(BinaryTreeNode<int>? root)
    {
        if (root is null)
        {
            return true;
        }

        var heightDifference = Math.Abs(Height(root.Left) - Height(root.Right));

        return heightDifference <= MaxAllowedHeightDifference
            && IsBalancedByTopDownHeightCheck(root.Left)
            && IsBalancedByTopDownHeightCheck(root.Right);
    }

    // A subtree's height on its own: one plus the taller child, with an empty tree at
    // zero. This is the O(n) measure the bottom-up arm folds into the balance check and
    // this arm pays once per ancestor.
    private static int Height(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    public static bool IsBalancedByHeightRecursion(BinaryTreeNode<int>? root) =>
        HeightOrUnbalanced(root) >= 0;

    private static int HeightOrUnbalanced(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return 0;
        }

        var leftHeight = HeightOrUnbalanced(node.Left);
        var rightHeight = HeightOrUnbalanced(node.Right);

        if (IsUnbalanced(leftHeight, rightHeight))
        {
            return UnbalancedHeightMarker;
        }

        return 1 + Math.Max(leftHeight, rightHeight);
    }

    // This node's subtree is unbalanced when either child already reported the marker,
    // or the two heights it just handed back differ by more than the one allowed.
    private static bool IsUnbalanced(int leftHeight, int rightHeight) =>
        leftHeight < 0 || rightHeight < 0 || Math.Abs(leftHeight - rightHeight) > MaxAllowedHeightDifference;
}
