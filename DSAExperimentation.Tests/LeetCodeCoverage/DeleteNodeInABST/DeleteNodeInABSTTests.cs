using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteNodeInABST;

// LeetCode 450. Delete Node in a BST: this repo's own BinarySearchTree<TValue> is
// exactly the target API - Insert builds the input tree, TryDelete removes the
// target key (leaf, one-child, and two-child cases - the latter via in-order-
// successor promotion - are all handled internally, see BinarySearchTree.cs's own
// DeleteFoundNode), and Has plus InOrderTraversal/IInOrderHooks (the same
// composition KthSmallestElementInABSTTests already uses) confirm the result is
// still a validly ordered BST with the key gone and every other key intact.
public sealed partial class DeleteNodeInABSTTests
{
    [Fact]
    public void TryDelete_LeafNode_RemovesItAndKeepsRemainingKeysOrdered()
    {
        var tree = BuildTree([5, 3, 6, 2, 4, 7]);

        var deleted = tree.TryDelete(2);

        Assert.True(deleted);
        Assert.False(tree.Has(2));
        Assert.Equal([3, 4, 5, 6, 7], InOrderValues(tree));
    }

    [Fact]
    public void TryDelete_NodeWithTwoChildren_PromotesInOrderSuccessorAndStaysOrdered()
    {
        var tree = BuildTree([5, 3, 6, 2, 4, 7]);

        var deleted = tree.TryDelete(3);

        Assert.True(deleted);
        Assert.False(tree.Has(3));
        Assert.Equal([2, 4, 5, 6, 7], InOrderValues(tree));
    }

    [Fact]
    public void TryDelete_ValueNotPresent_ReturnsFalseAndLeavesTreeUnchanged()
    {
        var tree = BuildTree([5, 3, 6, 2, 4, 7]);

        var deleted = tree.TryDelete(100);

        Assert.False(deleted);
        Assert.Equal([2, 3, 4, 5, 6, 7], InOrderValues(tree));
    }

    private static BinarySearchTree<int> BuildTree(int[] values)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        return tree;
    }

    private static int[] InOrderValues(BinarySearchTree<int> tree)
    {
        State.Values.Value = [];
        InOrderTraversal.Walk<int, CollectHooks>(tree.Root);
        return [.. State.Values.Value!];
    }

    private readonly struct CollectHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth) => State.Values.Value!.Add(node.Value);
    }

    private static class State
    {
        public static readonly AsyncLocal<List<int>?> Values = new();
    }
}
