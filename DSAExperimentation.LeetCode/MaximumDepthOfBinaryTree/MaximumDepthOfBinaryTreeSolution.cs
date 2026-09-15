using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MaximumDepthOfBinaryTree;

// LeetCode 104. Maximum Depth of Binary Tree: the number of nodes along the
// longest path from the root down to the farthest leaf.
//
// The two strategies compute the identical 1 + max(left, right) recurrence -
// one written out by hand, the other composed from this repo's own
// TreeFold-based TreeMetrics.Height, generic over any ITreeTopology.
internal static class MaximumDepthOfBinaryTreeSolution
{
    // Textbook baseline: plain recursion, stopping at null.
    public static int MaxDepthByRecursion(BinaryTreeNode<int>? root) =>
        root is null ? 0 : DepthBelow(root);

    // One level, plus whichever subtree reaches deeper.
    private static int DepthBelow(BinaryTreeNode<int> node)
        => 1 + Math.Max(MaxDepthByRecursion(node.Left), MaxDepthByRecursion(node.Right));

    // This repo's own height metric.
    public static int MaxDepthByTreeMetrics(BinaryTreeNode<int>? root) =>
        TreeMetrics.Height<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root);
}
