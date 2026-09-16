using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SerializeAndDeserializeBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SerializeAndDeserializeBinaryTree;

// Harness only. Both round-trip strategies are
// SerializeAndDeserializeBinaryTreeSolution's - this file just pins them to
// LeetCode's published examples via a preorder-sequence comparison, since the
// restored tree only needs to be structurally identical, not reference-equal.
public sealed partial class SerializeAndDeserializeBinaryTreeTests
{
    // Each row is a preorder traversal with null children marked (null), matching
    // the solution's own token grammar - value, then left, then right.
    public static TheoryData<int?[]> Examples =>
        new()
        {
            new int?[] { 1, 2, null, null, 3, 4, null, null, 5, null, null },
            new int?[] { null },
            new int?[] { 42, null, null },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SerializeByStringConcatThenDeserializeByStringConcat_LeetCodeExamples_RoundTripsPreOrder(
        int?[] preorder)
    {
        var root = BuildTree(preorder);

        var restored = SerializeAndDeserializeBinaryTreeSolution.DeserializeByStringConcat(
            SerializeAndDeserializeBinaryTreeSolution.SerializeByStringConcat(root));

        Assert.Equal(PreOrder(root), PreOrder(restored));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SerializeByQueueThenDeserializeByQueue_LeetCodeExamples_RoundTripsPreOrder(int?[] preorder)
    {
        var root = BuildTree(preorder);

        var restored = SerializeAndDeserializeBinaryTreeSolution.DeserializeByQueue(
            SerializeAndDeserializeBinaryTreeSolution.SerializeByQueue(root));

        Assert.Equal(PreOrder(root), PreOrder(restored));
    }

    private static BinaryTreeNode<int>? BuildTree(int?[] preorder)
    {
        // The read cursor is the recursion's own state, not this body's, so the local
        // function names it as a parameter rather than capturing it from the enclosing block.
        var index = 0;

        return BuildFromPreorder(ref index);

        BinaryTreeNode<int>? BuildFromPreorder(ref int index)
        {
            var value = preorder[index++];
            return value is null ? null : new BinaryTreeNode<int>(value.Value) { Left = BuildFromPreorder(ref index), Right = BuildFromPreorder(ref index) };
        }
    }

    private static int[] PreOrder(BinaryTreeNode<int>? root)
    {
        if (root is null)
        {
            return [];
        }

        return [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];
    }
}
