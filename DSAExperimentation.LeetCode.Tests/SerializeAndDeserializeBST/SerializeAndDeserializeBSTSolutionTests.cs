using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.SerializeAndDeserializeBST;

namespace DSAExperimentation.LeetCode.Tests.SerializeAndDeserializeBST;

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
    public void SerializeByNullMarkerQueue_LeetCodeExamples_RoundTripsThroughDeserializeByNullMarkerQueue(int?[] levelOrder)
    {
        var text = SerializeAndDeserializeBSTSolution.SerializeByNullMarkerQueue(LeetCodeWireFormat.ToBinaryTree(levelOrder));

        Assert.Equal(levelOrder, LeetCodeWireFormat.FromBinaryTree(SerializeAndDeserializeBSTSolution.DeserializeByNullMarkerQueue(text)));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeserializeByNullMarkerQueue_WhatSerializeByNullMarkerQueueWrote_WritesBackTheSameText(int?[] levelOrder)
    {
        var text = SerializeAndDeserializeBSTSolution.SerializeByNullMarkerQueue(LeetCodeWireFormat.ToBinaryTree(levelOrder));

        Assert.Equal(text, SerializeAndDeserializeBSTSolution.SerializeByNullMarkerQueue(SerializeAndDeserializeBSTSolution.DeserializeByNullMarkerQueue(text)));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SerializeByPreOrderValues_LeetCodeExamples_RoundTripsThroughDeserializeByBstInsert(int?[] levelOrder)
    {
        var text = SerializeAndDeserializeBSTSolution.SerializeByPreOrderValues(LeetCodeWireFormat.ToBinaryTree(levelOrder));

        Assert.Equal(levelOrder, LeetCodeWireFormat.FromBinaryTree(SerializeAndDeserializeBSTSolution.DeserializeByBstInsert(text)));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeserializeByBstInsert_WhatSerializeByPreOrderValuesWrote_WritesBackTheSameText(int?[] levelOrder)
    {
        var text = SerializeAndDeserializeBSTSolution.SerializeByPreOrderValues(LeetCodeWireFormat.ToBinaryTree(levelOrder));

        Assert.Equal(text, SerializeAndDeserializeBSTSolution.SerializeByPreOrderValues(SerializeAndDeserializeBSTSolution.DeserializeByBstInsert(text)));
    }
}
