using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class NotationReaderTests
{
    // 2.5 is exact in binary, so any digit count holds it; the comparison still states one.
    private const int FractionDigits = 12;

    [Fact]
    public void Read_Integer_ReadsTheNumber() => Assert.Equal(42, NotationReader.Read("42", typeof(int)));

    [Fact]
    public void Read_IntegerPastInt32_ReadsALong() => Assert.Equal(9_000_000_000L, NotationReader.Read("9000000000", typeof(long)));

    [Fact]
    public void Read_Fraction_ReadsADouble() =>
        Assert.Equal(2.5, Assert.IsType<double>(NotationReader.Read("2.5", typeof(double))), FractionDigits);

    [Fact]
    public void Read_Boolean_ReadsTheBoolean() => Assert.Equal(true, NotationReader.Read("true", typeof(bool)));

    [Fact]
    public void Read_QuotedText_ReadsTheTextWithoutItsQuotes() => Assert.Equal("leet", NotationReader.Read("\"leet\"", typeof(string)));

    [Fact]
    public void Read_OneCharacterText_ReadsAChar() => Assert.Equal('a', NotationReader.Read("\"a\"", typeof(char)));

    [Fact]
    public void Read_LongerTextAsAChar_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() => NotationReader.Read("\"ab\"", typeof(char)));

    [Fact]
    public void Read_NestedArray_ReadsEveryRow() =>
        Assert.Equal([[1, 3], [2, 6]], Assert.IsType<int[][]>(NotationReader.Read("[[1,3],[2,6]]", typeof(int[][]))));

    [Fact]
    public void Read_RowsAsPairs_ReadsEachRowAsATuple() =>
        Assert.Equal([(1, 3), (2, 6)], Assert.IsType<(int, int)[]>(NotationReader.Read("[[1,3],[2,6]]", typeof((int, int)[]))));

    [Fact]
    public void Read_RowOfTheWrongLengthAsAPair_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() => NotationReader.Read("[1,3,5]", typeof((int, int))));

    [Fact]
    public void Read_SequenceAStrategyOnlyReads_ReadsAnArray() =>
        Assert.IsType<int[]>(NotationReader.Read("[1,2]", typeof(IEnumerable<int>)));

    [Fact]
    public void Read_SequenceAStrategyMayAddTo_ReadsAList() =>
        Assert.IsType<List<int>>(NotationReader.Read("[1,2]", typeof(IList<int>)));

    [Fact]
    public void Read_LevelOrderTree_BuildsTheTreeItDescribes() =>
        Assert.Equal([1, null, 2, 3], LeetCodeWireFormat.FromBinaryTree((BinaryTreeNode<int>?)NotationReader.Read("[1,null,2,3]", typeof(BinaryTreeNode<int>))));

    [Fact]
    public void Read_EmptyTree_ReadsNoRoot() => Assert.Null(NotationReader.Read("[]", typeof(BinaryTreeNode<int>)));

    [Fact]
    public void Read_FlatArrayAsALinkedList_LinksTheValuesInOrder() =>
        Assert.Equal([1, 2, 3], LeetCodeWireFormat.FromLinkedList((SinglyLinkedListNode<int>?)NotationReader.Read("[1,2,3]", typeof(SinglyLinkedListNode<int>))));

    [Fact]
    public void Read_TypeOutsideTheRules_ThrowsNotSupportedException() =>
        Assert.Throws<NotSupportedException>(() => NotationReader.Read("{}", typeof(Dictionary<int, int>)));

    [Fact]
    public void CanRead_TypeReadTakes_IsTrue() =>
        Assert.True(NotationReader.CanRead(typeof(IEnumerable<(int, int)>)));

    [Fact]
    public void CanRead_TypeReadRefuses_IsFalse() => Assert.False(NotationReader.CanRead(typeof(Dictionary<int, int>)));

    [Fact]
    public void CanRead_RectangularArray_IsFalse() => Assert.False(NotationReader.CanRead(typeof(int[,])));
}
