using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.BalancedBinaryTree;

// LeetCode 110. Balanced Binary Tree: is every node's two subtree heights
// within one of each other?
//
// There is only one strategy here: the original test's private helper and
// both of the original benchmark's [Benchmark] arms all called the exact
// same bottom-up recursion, which folds height computation and the balance
// check into a single pass by returning -1 the moment a subtree is found
// unbalanced, short-circuiting the rest of the walk.
internal static class BalancedBinaryTreeSolution
{
    private const int UnbalancedHeightMarker = -1;
    private const int MaxAllowedHeightDifference = 1;

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

        if (leftHeight < 0 || rightHeight < 0 || Math.Abs(leftHeight - rightHeight) > MaxAllowedHeightDifference)
        {
            return UnbalancedHeightMarker;
        }

        return 1 + Math.Max(leftHeight, rightHeight);
    }
}
