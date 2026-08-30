using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertIntoABinarySearchTree;

// LeetCode 701. Insert into a Binary Search Tree: this repo's own
// BinarySearchTree<TValue>.Insert already is the target operation - the same
// find-the-empty-slot-and-attach walk this problem asks for, with LeetCode's own
// "any valid resulting BST is accepted" relaxation meaning no adapter is needed
// beyond checking the value landed and the tree is still validly ordered
// afterward (InOrderTraversal/IInOrderHooks, the same composition
// DeleteNodeInABSTTests already uses for its own post-mutation check).
public sealed partial class InsertIntoABinarySearchTreeTests
{
    [Fact]
    public void Insert_NewLeafValue_IsFindableAndKeepsRemainingKeysOrdered()
    {
        var tree = BuildTree([4, 2, 7, 1, 3]);

        tree.Insert(5);

        Assert.True(tree.Has(5));
        Assert.Equal([1, 2, 3, 4, 5, 7], InOrderValues(tree));
    }

    [Fact]
    public void Insert_IntoEmptyTree_BecomesTheRoot()
    {
        var tree = new BinarySearchTree<int>();

        tree.Insert(42);

        Assert.NotNull(tree.Root);
        Assert.Equal(42, tree.Root!.Value);
        Assert.Equal(1, tree.Count);
    }

    [Fact]
    public void Insert_ValueSmallerThanEveryExistingKey_BecomesLeftmostLeaf()
    {
        var tree = BuildTree([4, 2, 7]);

        tree.Insert(1);

        Assert.Equal([1, 2, 4, 7], InOrderValues(tree));
        Assert.Equal(1, tree.Root!.Left!.Left!.Value);
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
