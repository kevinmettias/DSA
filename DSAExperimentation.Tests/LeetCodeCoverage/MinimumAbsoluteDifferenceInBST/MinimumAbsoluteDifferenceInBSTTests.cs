using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAbsoluteDifferenceInBST;

// LeetCode 530. Minimum Absolute Difference in BST: an in-order walk of a BST
// visits values in ascending order, so the minimum absolute difference between any
// two nodes is always between some adjacent pair in that walk - this repo's own
// InOrderTraversal/IInOrderHooks over BinaryTreeNode<int>, the same "track prev
// across Visit calls via AsyncLocal state" composition RecoverBinarySearchTreeTests
// already uses, not a hand-rolled recursive walk.
public sealed partial class MinimumAbsoluteDifferenceInBSTTests
{
    [Fact]
    public void GetMinimumDifference_ThreeNodeTree_ReturnsOne()
    {
        // [1,null,3,2] -> in-order 1,2,3 -> min diff 1
        var root = new BinaryTreeNode<int>(1) { Right = new(3) { Left = new(2) } };

        Assert.Equal(1, GetMinimumDifference(root));
    }

    [Fact]
    public void GetMinimumDifference_LargerTree_ReturnsSmallestAdjacentGap()
    {
        // [4,2,6,1,3] -> in-order 1,2,3,4,6 -> min diff 1
        var root = new BinaryTreeNode<int>(4)
        {
            Left = new(2) { Left = new(1), Right = new(3) },
            Right = new(6),
        };

        Assert.Equal(1, GetMinimumDifference(root));
    }

    private static int GetMinimumDifference(BinaryTreeNode<int> root)
    {
        State.Prev.Value = null;
        State.MinDiff.Value = int.MaxValue;

        InOrderTraversal.Walk<int, DiffHooks>(root);

        return State.MinDiff.Value;
    }

    private readonly struct DiffHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.Prev.Value is { } prev)
            {
                State.MinDiff.Value = Math.Min(State.MinDiff.Value, node.Value - prev.Value);
            }

            State.Prev.Value = node;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<BinaryTreeNode<int>?> Prev = new();
        public static readonly AsyncLocal<int> MinDiff = new();
    }
}
