using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SerializeAndDeserializeBST;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SerializeAndDeserializeBST;

// Harness only. Both round-trip strategies are SerializeAndDeserializeBSTSolution's
// - this file just pins them to LeetCode's published examples via a preorder-value
// sequence, building each example tree through this repo's own
// BinarySearchTree<int>.Insert so its shape is always genuinely BST-ordered, and
// comparing round-tripped shape by preorder traversal since the restored tree only
// needs to be structurally identical, not reference-equal.
public sealed class SerializeAndDeserializeBSTTests
{
    public static TheoryData<int[]> Examples =>
        new()
        {
            new int[] { },
            new int[] { 42 },
            new int[] { 5, 3, 2, 4, 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SerializeByNullMarkerQueueThenDeserializeByNullMarkerQueue_LeetCodeExamples_RoundTripsPreOrder(
        int[] preOrder)
    {
        var root = BuildTree(preOrder);

        var restored = SerializeAndDeserializeBSTSolution.DeserializeByNullMarkerQueue(
            SerializeAndDeserializeBSTSolution.SerializeByNullMarkerQueue(root));

        Assert.Equal(PreOrder(root), PreOrder(restored));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SerializeByPreOrderValuesThenDeserializeByBstInsert_LeetCodeExamples_RoundTripsPreOrder(
        int[] preOrder)
    {
        var root = BuildTree(preOrder);

        var restored = SerializeAndDeserializeBSTSolution.DeserializeByBstInsert(
            SerializeAndDeserializeBSTSolution.SerializeByPreOrderValues(root));

        Assert.Equal(PreOrder(root), PreOrder(restored));
    }

    private static BinaryTreeNode<int>? BuildTree(int[] preOrder)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in preOrder)
        {
            tree.Insert(value);
        }

        return tree.Root;
    }

    private static int[] PreOrder(BinaryTreeNode<int>? root)
        => root is null ? [] : [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];
}
