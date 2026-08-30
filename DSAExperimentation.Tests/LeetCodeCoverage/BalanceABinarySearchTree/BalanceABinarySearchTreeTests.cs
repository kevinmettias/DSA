using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BalanceABinarySearchTree;

// LeetCode 1382. Balance a Binary Search Tree: an in-order walk of any BST visits
// values in ascending order regardless of shape, so collecting them via this
// repo's own InOrderTraversal/IInOrderHooks - the same composition FindModeInBina
// rySearchTreeTests/KthSmallestElementInABSTTests already use, into a
// DynamicArray<int> instead of a BCL List<int> - and rebuilding by always
// splitting at the midpoint (exactly ConvertSortedArrayToBinarySearchTreeTests'
// own Build helper) produces a height-balanced BST with the same values, no
// matter how skewed the input was.
public sealed partial class BalanceABinarySearchTreeTests
{
    [Fact]
    public void Balance_SkewedRightOnlyChain_ProducesHeightBalancedBst()
    {
        // [1,null,2,null,3,null,4] - a right-only chain, height 4.
        var root = new BinaryTreeNode<int>(1) { Right = new(2) { Right = new(3) { Right = new(4) } } };

        var balanced = Balance(root);

        Assert.Equal([1, 2, 3, 4], InOrder(balanced));
        Assert.True(Height(balanced) <= 3);
    }

    [Fact]
    public void Balance_AlreadyBalancedTree_PreservesValuesAndStaysBalanced()
    {
        var root = new BinaryTreeNode<int>(2) { Left = new(1), Right = new(3) };

        var balanced = Balance(root);

        Assert.Equal([1, 2, 3], InOrder(balanced));
        Assert.True(Math.Abs(Height(balanced!.Left) - Height(balanced.Right)) <= 1);
    }

    [Fact]
    public void Balance_SingleNode_ReturnsThatSameSingleNode()
    {
        var root = new BinaryTreeNode<int>(9);

        var balanced = Balance(root);

        Assert.Equal([9], InOrder(balanced));
        Assert.Equal(1, Height(balanced));
    }

    private static BinaryTreeNode<int>? Balance(BinaryTreeNode<int> root)
    {
        State.Sorted.Value = new DynamicArray<int>();

        InOrderTraversal.Walk<int, CollectHooks>(root);

        var sorted = State.Sorted.Value;
        return Build(sorted, 0, sorted.Count - 1);
    }

    private static BinaryTreeNode<int>? Build(DynamicArray<int> sorted, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / 2);
        return new BinaryTreeNode<int>(sorted.Get(mid))
        {
            Left = Build(sorted, low, mid - 1),
            Right = Build(sorted, mid + 1, high),
        };
    }

    private static int[] InOrder(BinaryTreeNode<int>? node)
        => node is null ? [] : [.. InOrder(node.Left), node.Value, .. InOrder(node.Right)];

    private static int Height(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));

    private readonly struct CollectHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth) => State.Sorted.Value!.Add(node.Value);
    }

    private static class State
    {
        public static readonly AsyncLocal<DynamicArray<int>> Sorted = new();
    }
}
