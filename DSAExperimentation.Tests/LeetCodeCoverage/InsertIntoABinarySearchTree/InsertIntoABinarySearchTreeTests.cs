using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.InsertIntoABinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertIntoABinarySearchTree;

// Harness only. Both strategies are InsertIntoABinarySearchTreeSolution's; this
// file pins them to LeetCode's published examples. LeetCode accepts ANY valid
// resulting BST, so assertions check the in-order sequence (every original
// value present, in order, plus the new one) rather than one particular shape.
public sealed class InsertIntoABinarySearchTreeTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [4, 2, 7, 1, 3], 5, [1, 2, 3, 4, 5, 7] },
            { [], 42, [42] },
            { [4, 2, 7], 1, [1, 2, 4, 7] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void InsertByCollectSortRebuild_LeetCodeExamples_ReturnsBstContainingNewValue(
        int[] existingValues, int newValue, int[] expectedInOrder)
    {
        var root = InsertIntoABinarySearchTreeSolution.InsertByCollectSortRebuild(existingValues, newValue);

        Assert.Equal(expectedInOrder, InOrderValues(root));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void InsertByBstInsert_LeetCodeExamples_ReturnsBstContainingNewValue(
        int[] existingValues, int newValue, int[] expectedInOrder)
    {
        var root = InsertIntoABinarySearchTreeSolution.InsertByBstInsert(existingValues, newValue);

        Assert.Equal(expectedInOrder, InOrderValues(root));
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
