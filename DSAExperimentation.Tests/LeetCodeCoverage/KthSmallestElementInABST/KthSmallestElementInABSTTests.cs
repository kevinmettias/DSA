using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSmallestElementInABST;

// LeetCode 230. Kth Smallest Element in a BST: an in-order walk of a BST visits
// values in ascending order, so the kth value visited is the answer - this repo's
// own InOrderTraversal/IInOrderHooks over BinaryTreeNode<int>, the same
// composition RecoverBinarySearchTreeTests already uses, not a hand-rolled
// recursive walk. IInOrderHooks.Visit has no early-exit signal, so the hook keeps
// counting down past the found node and simply stops overwriting the result once
// State.Result.Value is set.
public sealed partial class KthSmallestElementInABSTTests
{
    [Fact]
    public void KthSmallest_FirstRank_ReturnsMinimumValue()
    {
        // [3,1,4,null,2], k = 1 -> 1
        var root = new BinaryTreeNode<int>(3) { Left = new(1) { Right = new(2) }, Right = new(4) };

        var actual = KthSmallest(root, 1);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void KthSmallest_ThirdRankInLargerTree_ReturnsInOrderValue()
    {
        // [5,3,6,2,4,null,null,1], k = 3 -> 3
        var root = new BinaryTreeNode<int>(5)
        {
            Left = new(3) { Left = new(2) { Left = new(1) }, Right = new(4) },
            Right = new(6),
        };

        var actual = KthSmallest(root, 3);
        Assert.Equal(3, actual);
    }

    private static int KthSmallest(BinaryTreeNode<int> root, int k)
    {
        State.Remaining.Value = k;
        State.Result.Value = null;

        InOrderTraversal.Walk<int, RankHooks>(root);

        // Both Facts above pass a k no larger than their tree's node count, so the walk
        // reaches the kth visit and RankHooks sets Result before returning.
        return State.Result.Value
            ?? throw new InvalidOperationException(
                $"the tree has fewer than {k} nodes, so the in-order walk never reached rank {k}");
    }

    private readonly struct RankHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.Result.Value is not null)
            {
                return;
            }

            State.Remaining.Value--;

            if (State.Remaining.Value == 0)
            {
                State.Result.Value = node.Value;
            }
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<int> Remaining = new();
        public static readonly AsyncLocal<int?> Result = new();
    }
}
