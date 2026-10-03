using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.SerializeAndDeserializeBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SerializeAndDeserializeBinaryTree;

// Harness only. Both codecs are SerializeAndDeserializeBinaryTreeSolution's. LeetCode
// leaves the encoding free and judges only the round trip, so each codec is pinned
// from both ends: a tree written and read back is the same tree, compared in
// LeetCode's own level-order notation (which, unlike a value-only preorder, tells
// two shapes with the same values apart); and text read and written back is the
// same text.
public sealed partial class SerializeAndDeserializeBinaryTreeTests
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
    public void SerializeByStringConcat_LeetCodeExamples_RoundTripsThroughDeserializeByStringConcat(int?[] levelOrder)
    {
        var text = SerializeAndDeserializeBinaryTreeSolution.SerializeByStringConcat(LeetCodeWireFormat.ToBinaryTree(levelOrder));

        Assert.Equal(levelOrder, LeetCodeWireFormat.FromBinaryTree(SerializeAndDeserializeBinaryTreeSolution.DeserializeByStringConcat(text)));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeserializeByStringConcat_WhatSerializeByStringConcatWrote_WritesBackTheSameText(int?[] levelOrder)
    {
        var text = SerializeAndDeserializeBinaryTreeSolution.SerializeByStringConcat(LeetCodeWireFormat.ToBinaryTree(levelOrder));

        Assert.Equal(text, SerializeAndDeserializeBinaryTreeSolution.SerializeByStringConcat(SerializeAndDeserializeBinaryTreeSolution.DeserializeByStringConcat(text)));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SerializeByQueue_LeetCodeExamples_RoundTripsThroughDeserializeByQueue(int?[] levelOrder)
    {
        var text = SerializeAndDeserializeBinaryTreeSolution.SerializeByQueue(LeetCodeWireFormat.ToBinaryTree(levelOrder));

        Assert.Equal(levelOrder, LeetCodeWireFormat.FromBinaryTree(SerializeAndDeserializeBinaryTreeSolution.DeserializeByQueue(text)));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeserializeByQueue_WhatSerializeByQueueWrote_WritesBackTheSameText(int?[] levelOrder)
    {
        var text = SerializeAndDeserializeBinaryTreeSolution.SerializeByQueue(LeetCodeWireFormat.ToBinaryTree(levelOrder));

        Assert.Equal(text, SerializeAndDeserializeBinaryTreeSolution.SerializeByQueue(SerializeAndDeserializeBinaryTreeSolution.DeserializeByQueue(text)));
    }
}
