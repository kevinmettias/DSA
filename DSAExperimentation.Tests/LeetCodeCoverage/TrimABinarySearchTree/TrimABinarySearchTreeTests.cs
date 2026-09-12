using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.TrimABinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TrimABinarySearchTree;

// Harness only. Both strategies are TrimABinarySearchTreeSolution's; this file
// builds LeetCode's published examples via this repo's own
// BinarySearchTree<int>.Insert and confirms what survives via
// InOrderTraversal/IInOrderHooks (the same composition DeleteNodeInABSTTests
// already uses), which also confirms the surviving values stay sorted.
public sealed class TrimABinarySearchTreeTests
{
    public static TheoryData<int[], int, int, int[]> Examples =>
        new()
        {
            { [3, 0, 4, 2, 1], 1, 3, [1, 2, 3] },
            { [1, 0, 2], 3, 5, [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TrimByInPlaceMutation_VariousRanges_DropsNodesOutsideRange(
        int[] values, int low, int high, int[] expected)
    {
        var tree = BuildTree(values);

        var trimmed = TrimABinarySearchTreeSolution.TrimByInPlaceMutation(tree.Root, low, high);

        Assert.Equal(expected, InOrderValues(trimmed));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TrimByCollectAndRebuild_VariousRanges_DropsNodesOutsideRange(
        int[] values, int low, int high, int[] expected)
    {
        var tree = BuildTree(values);

        var trimmed = TrimABinarySearchTreeSolution.TrimByCollectAndRebuild(tree.Root, low, high);

        Assert.Equal(expected, InOrderValues(trimmed));
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
