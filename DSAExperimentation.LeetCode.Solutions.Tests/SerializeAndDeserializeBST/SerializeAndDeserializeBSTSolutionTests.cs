using DSAExperimentation.LeetCode.Solutions.SerializeAndDeserializeBST;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SerializeAndDeserializeBST;

// Harness only. Both codecs are SerializeAndDeserializeBSTSolution's. LeetCode leaves
// the encoding free and judges only the round trip, so each codec is pinned from both
// ends: a tree written and read back is the same tree, compared in LeetCode's own
// level-order notation; and text read and written back is the same text. Every row is
// a valid BST, which the values-only codec relies on.
public sealed partial class SerializeAndDeserializeBSTSolutionTests
{
    // LeetCode's two published examples, a single node, and a two-level tree.
    public static TheoryData<int?[]> Examples =>
        new()
        {
            { [2, 1, 3] },
            { [] },
            { [42] },
            { [5, 3, 6, 2, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RoundTripByNullMarkerQueue_LeetCodeExamples_ReturnsTheSameTree(int?[] levelOrder)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(levelOrder);
        var copy = SerializeAndDeserializeBSTSolution.RoundTripByNullMarkerQueue(root);

        Assert.Equal(levelOrder, LeetCodeWireFormat.FromBinaryTree(copy));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NullMarkerQueueCodec_WhatSerializeWrote_DeserializesAndSerializesBackToTheSameText(int?[] levelOrder)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(levelOrder);
        var text = SerializeAndDeserializeBSTSolution.NullMarkerQueueCodec.Serialize(root);
        var copy = SerializeAndDeserializeBSTSolution.NullMarkerQueueCodec.Deserialize(text);

        Assert.Equal(text, SerializeAndDeserializeBSTSolution.NullMarkerQueueCodec.Serialize(copy));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RoundTripByPreOrderValues_LeetCodeExamples_ReturnsTheSameTree(int?[] levelOrder)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(levelOrder);
        var copy = SerializeAndDeserializeBSTSolution.RoundTripByPreOrderValues(root);

        Assert.Equal(levelOrder, LeetCodeWireFormat.FromBinaryTree(copy));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PreOrderValuesCodec_WhatSerializeWrote_DeserializesAndSerializesBackToTheSameText(int?[] levelOrder)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(levelOrder);
        var text = SerializeAndDeserializeBSTSolution.PreOrderValuesCodec.Serialize(root);
        var copy = SerializeAndDeserializeBSTSolution.PreOrderValuesCodec.Deserialize(text);

        Assert.Equal(text, SerializeAndDeserializeBSTSolution.PreOrderValuesCodec.Serialize(copy));
    }
}
