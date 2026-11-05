using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class AnswerNotationTests
{
    [Fact]
    public void Of_Integer_WritesTheNumber() => Assert.Equal("42", AnswerNotation.Of(42).ToJsonString());

    [Fact]
    public void Of_UnsignedInteger_WritesTheNumber() => Assert.Equal("964176192", AnswerNotation.Of(964176192u).ToJsonString());

    [Fact]
    public void Of_Character_WritesAOneCharacterString() => Assert.Equal("\"a\"", AnswerNotation.Of('a').ToJsonString());

    [Fact]
    public void Of_Tree_WritesItsLevelOrder() =>
        Assert.Equal("[1,null,2]", AnswerNotation.Of(LeetCodeWireFormat.ToBinaryTree([1, null, 2])).ToJsonString());

    [Fact]
    public void Of_NoTreeAtAll_WritesTheEmptyArray() => Assert.Equal("[]", AnswerNotation.Of((BinaryTreeNode<int>?)null).ToJsonString());

    // A sequence of lists writes a missing one as the empty list, as LeetCode prints LC 725's empty parts.
    [Fact]
    public void Of_ListsWithAMissingOne_WritesItAsTheEmptyArray()
    {
        SinglyLinkedListNode<int>?[] parts = [LeetCodeWireFormat.ToLinkedList([1]), null];

        Assert.Equal("[[1],[]]", AnswerNotation.Of(parts).ToJsonString());
    }

    // A level-order array's null stays LeetCode's null: a missing child, not a missing tree.
    [Fact]
    public void Of_LevelOrderWithAMissingChild_WritesNull() => Assert.Equal("[1,null,2]", AnswerNotation.Of(new int?[] { 1, null, 2 }).ToJsonString());

    [Fact]
    public void Of_ListOfPairs_WritesEachPairAsARow() =>
        Assert.Equal("[[1,6],[8,10]]", AnswerNotation.Of(new List<(int, int)> { (1, 6), (8, 10) }).ToJsonString());

    [Fact]
    public void OfFound_TwoIndices_WritesThemAsAnArray() => Assert.Equal("[0,1]", AnswerNotation.OfFound([0, 1]).ToJsonString());

    [Fact]
    public void NoneFound_AnyTryStrategy_WritesTheEmptyAnswer() => Assert.Equal("[]", AnswerNotation.NoneFound().ToJsonString());

    [Fact]
    public void OfFoundOutputs_TupleThatFoundNothing_WritesTheEmptyAnswer() =>
        Assert.Equal("[]", AnswerNotation.OfFoundOutputs((false, 0, 0)).ToJsonString());

    [Fact]
    public void OfFoundOutputs_GatheredTuple_WritesTheOutputsAfterTheFlag() =>
        Assert.Equal("[2,5]", AnswerNotation.OfFoundOutputs((true, 2, 5)).ToJsonString());

    [Fact]
    public void OfFoundOutputs_NotATuple_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() => AnswerNotation.OfFoundOutputs(7));
}
