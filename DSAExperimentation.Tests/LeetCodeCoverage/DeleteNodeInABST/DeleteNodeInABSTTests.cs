using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.DeleteNodeInABST;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteNodeInABST;

// Harness only. Both strategies live in DeleteNodeInABSTSolution; this file just
// pins them to LeetCode's own answer shape for LC 450 (the tree after deletion,
// read back via the same Has/InOrderTraversal composition KthSmallestElementInABST
// -Tests already uses) rather than the boolean BinarySearchTree.TryDelete happens
// to return.
public sealed class DeleteNodeInABSTTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [5, 3, 6, 2, 4, 7], 2, [3, 4, 5, 6, 7] },       // leaf
            { [5, 3, 6, 2, 4, 7], 3, [2, 4, 5, 6, 7] },       // two children: in-order successor promoted
            { [5, 3, 6, 2, 4, 7], 100, [2, 3, 4, 5, 6, 7] },  // key not present: every key survives
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteByCollectFilterRebuild_LeetCodeExamples_ReturnsTreeWithKeyRemoved(
        int[] values, int key, int[] expected) =>
        Assert.Equal(expected, InOrderValues(DeleteNodeInABSTSolution.DeleteByCollectFilterRebuild(BuildTree(values), key)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteByBinarySearchTreeDelete_LeetCodeExamples_ReturnsTreeWithKeyRemoved(
        int[] values, int key, int[] expected) =>
        Assert.Equal(expected, InOrderValues(DeleteNodeInABSTSolution.DeleteByBinarySearchTreeDelete(BuildTree(values), key)));

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
