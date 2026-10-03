using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.DeleteNodeInABST;

namespace DSAExperimentation.LeetCode.Tests.DeleteNodeInABST;

// Harness only. Both strategies live in DeleteNodeInABSTSolution; this file just
// pins them to LeetCode's own answer shape for LC 450 (the tree after deletion,
// read back via the same Has/InOrderTraversal composition KthSmallestElementInABST
// -Tests already uses) rather than the boolean BinarySearchTree.TryDelete happens
// to return.
public sealed partial class DeleteNodeInABSTSolutionTests
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
        int[] values, int key, int[] expected)
    {
        var tree = DeleteNodeInABSTSolution.DeleteByCollectFilterRebuild(BuildTree(values), key);
        var actual = InOrderValues(tree);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteByBinarySearchTreeDelete_LeetCodeExamples_ReturnsTreeWithKeyRemoved(
        int[] values, int key, int[] expected)
    {
        var tree = DeleteNodeInABSTSolution.DeleteByBinarySearchTreeDelete(BuildTree(values), key);
        var actual = InOrderValues(tree);
        Assert.Equal(expected, actual);
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
        var values = new List<int>();
        InOrderTraversal.Walk(tree.Root, new CollectHooks(values));
        return [.. values];
    }

    private readonly struct CollectHooks(List<int> values) : IInOrderHooks<int>
    {
        public void Visit(BinaryTreeNode<int> node, int depth) => values.Add(node.Value);
    }
}
