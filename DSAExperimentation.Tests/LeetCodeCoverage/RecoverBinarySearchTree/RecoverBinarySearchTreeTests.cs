using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RecoverBinarySearchTree;

// LeetCode 99. Recover Binary Search Tree: exactly two nodes had their values
// swapped by mistake. An in-order walk of a correct BST is non-decreasing, so the
// two violation points (prev.Value > node.Value) pin down the misplaced pair -
// this repo's own InOrderTraversal/IInOrderHooks over BinaryTreeNode<int>, not a
// hand-rolled recursive walk.
public sealed partial class RecoverBinarySearchTreeTests
{
    [Fact]
    public void Recover_AdjacentSwapAtTheRoot_RestoresBstOrdering()
    {
        // [1,3,null,null,2] -> expected [3,1,null,null,2]
        var root = new BinaryTreeNode<int>(1) { Left = new(3) { Right = new(2) } };

        Recover(root);

        Assert.Equal(3, root.Value);
        Assert.Equal(1, root.Left!.Value);
        Assert.Equal(2, root.Left.Right!.Value);
    }

    [Fact]
    public void Recover_NonAdjacentSwapAcrossTheTree_RestoresBstOrdering()
    {
        // [3,1,4,null,null,2] -> expected [2,1,4,null,null,3]
        var root = new BinaryTreeNode<int>(3) { Left = new(1), Right = new(4) { Left = new(2) } };

        Recover(root);

        Assert.Equal(2, root.Value);
        Assert.Equal(1, root.Left!.Value);
        Assert.Equal(4, root.Right!.Value);
        Assert.Equal(3, root.Right.Left!.Value);
    }

    private static void Recover(BinaryTreeNode<int> root)
    {
        State.Prev.Value = null;
        State.First.Value = null;
        State.Second.Value = null;

        InOrderTraversal.Walk<int, ScanHooks>(root);

        var first = State.First.Value!;
        var second = State.Second.Value!;
        (first.Value, second.Value) = (second.Value, first.Value);
    }

    private readonly struct ScanHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.Prev.Value is { } prev && prev.Value > node.Value)
            {
                State.First.Value ??= prev;
                State.Second.Value = node;
            }

            State.Prev.Value = node;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<BinaryTreeNode<int>?> Prev = new();
        public static readonly AsyncLocal<BinaryTreeNode<int>?> First = new();
        public static readonly AsyncLocal<BinaryTreeNode<int>?> Second = new();
    }
}
