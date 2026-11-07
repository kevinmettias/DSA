using DSAExperimentation.LeetCode.Solutions.SerializeAndDeserializeBinaryTree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SerializeAndDeserializeBinaryTree;

// Harness only. Both codecs are SerializeAndDeserializeBinaryTreeSolution's. LeetCode
// leaves the encoding free and judges only the round trip, so each codec is pinned
// from both ends: a tree written and read back is the same tree, compared in
// LeetCode's own level-order notation (which, unlike a value-only preorder, tells
// two shapes with the same values apart); and text read and written back is the
// same text.
public sealed partial class SerializeAndDeserializeBinaryTreeSolutionTests
{
    // LeetCode's two published examples, a single node, and a lopsided tree whose
    // value-only preorder [1, 2, 3] it shares with a left-leaning chain.
    public static TheoryData<int?[]> Examples =>
        new()
        {
            { [1, 2, 3, null, null, 4, 5] },
            { [] },
            { [42] },
            { [1, null, 2, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RoundTripByStringConcat_LeetCodeExamples_ReturnsTheSameTree(int?[] levelOrder)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(levelOrder);
        var copy = SerializeAndDeserializeBinaryTreeSolution.RoundTripByStringConcat(root);

        Assert.Equal(levelOrder, LeetCodeWireFormat.FromBinaryTree(copy));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void StringConcatCodec_WhatSerializeWrote_DeserializesAndSerializesBackToTheSameText(int?[] levelOrder)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(levelOrder);
        var text = SerializeAndDeserializeBinaryTreeSolution.StringConcatCodec.Serialize(root);
        var copy = SerializeAndDeserializeBinaryTreeSolution.StringConcatCodec.Deserialize(text);

        Assert.Equal(text, SerializeAndDeserializeBinaryTreeSolution.StringConcatCodec.Serialize(copy));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RoundTripByQueue_LeetCodeExamples_ReturnsTheSameTree(int?[] levelOrder)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(levelOrder);
        var copy = SerializeAndDeserializeBinaryTreeSolution.RoundTripByQueue(root);

        Assert.Equal(levelOrder, LeetCodeWireFormat.FromBinaryTree(copy));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void QueueCodec_WhatSerializeWrote_DeserializesAndSerializesBackToTheSameText(int?[] levelOrder)
    {
        var root = LeetCodeWireFormat.ToBinaryTree(levelOrder);
        var text = SerializeAndDeserializeBinaryTreeSolution.QueueCodec.Serialize(root);
        var copy = SerializeAndDeserializeBinaryTreeSolution.QueueCodec.Deserialize(text);

        Assert.Equal(text, SerializeAndDeserializeBinaryTreeSolution.QueueCodec.Serialize(copy));
    }
}
