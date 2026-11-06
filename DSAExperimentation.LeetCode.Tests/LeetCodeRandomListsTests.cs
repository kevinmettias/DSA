using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Tests;

public sealed partial class LeetCodeRandomListsTests
{
    public static TheoryData<int?[][]> RandomLists =>
        new()
        {
            { [[7, null], [13, 0], [11, 4], [10, 2], [1, 0]] },
            { [[1, 1], [2, 1]] },
            { [] },
        };

    [Theory]
    [MemberData(nameof(RandomLists))]
    public void PairsOf_ListBuiltFromThePairs_ReturnsLeetCodesOwnPairs(int?[][] nodes)
    {
        var head = LeetCodeRandomLists.From(nodes);

        Assert.Equal(nodes, LeetCodeRandomLists.PairsOf(head));
    }

    [Fact]
    public void IsSharingANode_SeparateListsOfTheSamePairs_IsFalse()
    {
        var original = LeetCodeRandomLists.From([[1, 1], [2, 1]]);
        var copy = LeetCodeRandomLists.From([[1, 1], [2, 1]]);
        var isSharing = LeetCodeRandomLists.IsSharingANode(copy, original);

        Assert.False(isSharing);
    }

    [Fact]
    public void IsSharingANode_CopyWhoseRandomPointsIntoTheOriginal_IsTrue()
    {
        var original = LeetCodeRandomLists.From([[1, 1], [2, 1]]);
        var pairs = LeetCodeRandomLists.PairsOf(original);
        var copy = Assert.IsType<RandomLinkedListNode<int>>(LeetCodeRandomLists.From(pairs));
        copy.Random = original;
        var isSharing = LeetCodeRandomLists.IsSharingANode(copy, original);

        Assert.True(isSharing);
    }
}
