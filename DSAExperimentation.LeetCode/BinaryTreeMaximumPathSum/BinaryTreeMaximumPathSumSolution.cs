using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.BinaryTreeMaximumPathSum;

// LeetCode 124. Binary Tree Maximum Path Sum: the greatest sum along any
// downward-then-upward path through the tree - not necessarily through the
// root, and not necessarily ending at a leaf on either side.
//
// There is exactly one strategy: pre-migration the test's private helper was
// the only real implementation - the benchmark's two [Benchmark] arms were
// untested placeholders (`=> 1`), not a second arm to reconcile. Each node
// reports the best downward gain a parent may fold into its own value
// (clamped to zero, since a negative branch is worth skipping), while a
// running best total is updated with the path that bends through both
// children at once - a path no parent could ever extend upward.
internal static class BinaryTreeMaximumPathSumSolution
{
    public static int MaxPathSumByGainRecursion(BinaryTreeNode<int>? root)
    {
        var best = int.MinValue;
        Gain(root, ref best);

        return best;
    }

    private static int Gain(BinaryTreeNode<int>? node, ref int best)
    {
        if (node is null)
        {
            return 0;
        }

        var leftGain = Gain(node.Left, ref best);
        var left = Math.Max(0, leftGain);
        var rightGain = Gain(node.Right, ref best);
        var right = Math.Max(0, rightGain);

        best = Math.Max(best, node.Value + left + right);

        return node.Value + Math.Max(left, right);
    }
}
