using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MaximumSumBSTInBinaryTree;

// LeetCode 1373. Maximum Sum BST in Binary Tree: the largest sum of any subtree of
// this repo's own BinaryTreeNode<int> that is itself a valid binary search tree.
//
// Both strategies start their running best at LeetCode's empty-BST answer, zero:
// LC 1373's own third example ([-4,-2,-5] -> 0) settles that an all-negative tree
// reports 0 rather than its least-negative single node, because the empty subtree is
// itself a valid BST with sum 0. The pre-migration test and benchmark both started
// from int.MinValue instead and so would have answered -2 there; every example either
// of them actually asserted has a positive answer, so nothing they measured changes.
//
// TreeFold isn't a fit for either arm: BinaryTreeChildren compacts away a missing
// Left/Right (its own doc comment explains why), and this algorithm's every combine
// step needs to know specifically whether left.Max or right.Min came from an *actual*
// left/right child versus an absent one - exactly the positional identity that
// compaction throws away.
internal static class MaximumSumBSTInBinaryTreeSolution
{
    // The empty subtree is a valid BST, and its sum is the floor on any answer.
    private const int EmptySubtreeSum = 0;

    // The textbook answer: visit every node and, independently of everything already
    // computed, revalidate and re-sum its whole subtree from scratch. O(n) work at
    // each of n nodes, so O(n^2) on a tree whose subtrees really are BST-valid.
    // Deliberately written as plain recursion over the node type - it is the arm the
    // single-pass scan below has to justify itself against.
    public static int MaxSumBSTByRevalidatingEachNode(BinaryTreeNode<int>? root)
    {
        var best = EmptySubtreeSum;
        Visit(root);
        return best;

        void Visit(BinaryTreeNode<int>? node)
        {
            if (node is null)
            {
                return;
            }

            if (IsValidBst(node, null, null))
            {
                best = Math.Max(best, Sum(node));
            }

            Visit(node.Left);
            Visit(node.Right);
        }
    }

    private static bool IsValidBst(BinaryTreeNode<int>? node, int? min, int? max)
        => node is null || ((min is null || node.Value > min) && (max is null || node.Value < max)
            && IsValidBst(node.Left, min, node.Value) && IsValidBst(node.Right, node.Value, max));

    private static int Sum(BinaryTreeNode<int>? node)
        => node is null ? 0 : node.Value + Sum(node.Left) + Sum(node.Right);

    // One bottom-up post-order pass: each node combines its two already-computed
    // child summaries (IsBst, Min, Max, Sum) into its own, so every subtree is
    // validated and summed exactly once - the same "compute everything a node needs
    // from its two already-folded children" shape TreeMetrics/DiameterAlgebra use.
    public static int MaxSumBSTByBottomUpScan(BinaryTreeNode<int>? root)
    {
        var best = EmptySubtreeSum;
        Scan(root);
        return best;

        Summary Scan(BinaryTreeNode<int>? node)
        {
            if (node is null)
            {
                return new Summary(IsBst: true, Min: int.MaxValue, Max: int.MinValue, Sum: 0);
            }

            var left = Scan(node.Left);
            var right = Scan(node.Right);
            var isBst = left.IsBst && right.IsBst && node.Value > left.Max && node.Value < right.Min;

            if (!isBst)
            {
                return new Summary(false, 0, 0, 0);
            }

            var sum = left.Sum + right.Sum + node.Value;
            best = Math.Max(best, sum);

            return new Summary(true, Math.Min(node.Value, left.Min), Math.Max(node.Value, right.Max), sum);
        }
    }

    private readonly record struct Summary(bool IsBst, int Min, int Max, int Sum);
}
