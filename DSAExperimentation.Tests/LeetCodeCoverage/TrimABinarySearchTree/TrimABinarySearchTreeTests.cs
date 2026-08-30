using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TrimABinarySearchTree;

// LeetCode 669. Trim a Binary Search Tree: this repo's own BinarySearchTree<TValue>
// builds the input tree via Insert, then a recursive trim mutates Left/Right in
// place - exactly the reason BinaryTreeNode<TValue>'s own doc comment names this
// problem for why Left/Right stay settable. An out-of-range node is dropped by
// returning its in-range child's own trimmed result in its place, the same
// "return the replacement subtree root, caller reassigns node.Left/node.Right"
// shape BinarySearchTree.TryDelete already uses. InOrderTraversal/IInOrderHooks
// (the same composition DeleteNodeInABSTTests already uses) confirms what
// survives is still sorted and fully inside [low, high].
public sealed partial class TrimABinarySearchTreeTests
{
    [Fact]
    public void Trim_ClassicExample_DropsNodesOutsideRange()
    {
        var tree = BuildTree([3, 0, 4, 2, 1]);

        var trimmed = Trim(tree.Root, 1, 3);

        Assert.Equal([1, 2, 3], InOrderValues(trimmed));
    }

    [Fact]
    public void Trim_EntireRangeBelowLow_ReturnsEmptyTree()
    {
        var tree = BuildTree([1, 0, 2]);

        var trimmed = Trim(tree.Root, 3, 5);

        Assert.Null(trimmed);
    }

    private static BinaryTreeNode<int>? Trim(BinaryTreeNode<int>? node, int low, int high)
    {
        if (node is null)
        {
            return null;
        }

        if (node.Value < low)
        {
            return Trim(node.Right, low, high);
        }

        if (node.Value > high)
        {
            return Trim(node.Left, low, high);
        }

        node.Left = Trim(node.Left, low, high);
        node.Right = Trim(node.Right, low, high);
        return node;
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

    private static int[] InOrderValues(BinaryTreeNode<int>? root)
    {
        State.Values.Value = [];
        InOrderTraversal.Walk<int, CollectHooks>(root);
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
