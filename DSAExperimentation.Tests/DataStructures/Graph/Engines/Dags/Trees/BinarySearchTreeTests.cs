using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed partial class BinarySearchTreeTests
{
    private struct InsertOrderMarker;
    private struct OneChildMarker;
    private struct TwoChildMarker;

    [Fact]
    public void Count_StartsAtZeroOnAnEmptyTree()
        => Assert.Equal(0, new BinarySearchTree<int>().Count);

    [Fact]
    public void Root_EmptyTree_IsNull()
        => Assert.Null(new BinarySearchTree<int>().Root);

    [Fact]
    public void Root_AfterInserts_IsTheFirstInsertedValue()
    {
        var tree = new BinarySearchTree<int>();
        tree.Insert(5);
        tree.Insert(3);

        AssertRootValue(tree, 5);
    }

    [Fact]
    public void Insert_MultipleValues_ProducesSortedInOrderSequence()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in new[] { 5, 3, 8, 1, 4, 7, 9 })
        {
            tree.Insert(value);
        }

        InOrderTraversal.Walk<int, RecordingInOrderHooks<int, InsertOrderMarker>>(tree.Root);

        Assert.Equal(
            new[] { 1, 3, 4, 5, 7, 8, 9 },
            RecordingInOrderHooks<int, InsertOrderMarker>.Visited.Select(v => v.Value));
        Assert.Equal(7, tree.Count);
    }

    [Fact]
    public void Insert_DuplicateValue_IsNoOp()
    {
        var tree = new BinarySearchTree<int>();
        tree.Insert(5);

        tree.Insert(5);

        Assert.Equal(1, tree.Count);
    }

    [Fact]
    public void Has_ValuePresent_ReturnsTrue()
    {
        var tree = new BinarySearchTree<int>();
        tree.Insert(5);
        tree.Insert(3);

        Assert.True(tree.Has(3));
    }

    [Fact]
    public void Has_ValueAbsent_ReturnsFalse()
    {
        var tree = new BinarySearchTree<int>();
        tree.Insert(5);

        Assert.False(tree.Has(3));
    }

    [Fact]
    public void Has_EmptyTree_ReturnsFalse()
        => Assert.False(new BinarySearchTree<int>().Has(1));

    [Fact]
    public void TryDelete_LeafNode_RemovesIt()
    {
        var tree = new BinarySearchTree<int>();
        foreach (var value in new[] { 5, 3, 8 })
        {
            tree.Insert(value);
        }

        var removed = tree.TryDelete(3);

        Assert.True(removed);
        Assert.False(tree.Has(3));
        Assert.Equal(2, tree.Count);
    }

    // 5 -> Left=3 -> Right=4 (3 has exactly one child).
    [Fact]
    public void TryDelete_NodeWithOneChild_PromotesTheChild()
    {
        var tree = new BinarySearchTree<int>();
        foreach (var value in new[] { 5, 3, 4 })
        {
            tree.Insert(value);
        }

        tree.TryDelete(3);

        InOrderTraversal.Walk<int, RecordingInOrderHooks<int, OneChildMarker>>(tree.Root);
        Assert.Equal(new[] { 4, 5 }, RecordingInOrderHooks<int, OneChildMarker>.Visited.Select(v => v.Value));
        Assert.Equal(2, tree.Count);
    }

    // 5 -> Left=3, Right=8 -> Left=7, Right=9 (8's in-order successor is 7).
    [Fact]
    public void TryDelete_NodeWithTwoChildren_PromotesInOrderSuccessor()
    {
        var tree = new BinarySearchTree<int>();
        foreach (var value in new[] { 5, 3, 8, 7, 9 })
        {
            tree.Insert(value);
        }

        tree.TryDelete(5);

        InOrderTraversal.Walk<int, RecordingInOrderHooks<int, TwoChildMarker>>(tree.Root);
        Assert.Equal(
            new[] { 3, 7, 8, 9 },
            RecordingInOrderHooks<int, TwoChildMarker>.Visited.Select(v => v.Value));
        Assert.Equal(4, tree.Count);
        Assert.Equal(7, Assert.IsType<BinaryTreeNode<int>>(tree.Root).Value);
    }

    [Fact]
    public void TryDelete_RootWithNoChildren_EmptiesTree()
    {
        var tree = new BinarySearchTree<int>();
        tree.Insert(5);

        tree.TryDelete(5);

        Assert.Null(tree.Root);
        Assert.Equal(0, tree.Count);
    }

    [Fact]
    public void TryDelete_ValueNotPresent_ReturnsFalseAndLeavesTreeUnchanged()
    {
        var tree = new BinarySearchTree<int>();
        tree.Insert(5);

        var removed = tree.TryDelete(99);

        Assert.False(removed);
        Assert.Equal(1, tree.Count);
    }

    [Fact]
    public void TryDelete_EmptyTree_ReturnsFalse()
        => Assert.False(new BinarySearchTree<int>().TryDelete(1));

    // The first value inserted stays the root: a later insert hangs off it rather than
    // replacing it.
    private static void AssertRootValue(BinarySearchTree<int> tree, int expected)
    {
        var root = Assert.IsType<BinaryTreeNode<int>>(tree.Root);

        Assert.Equal(expected, root.Value);
    }
}
