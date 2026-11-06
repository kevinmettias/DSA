using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Harness.Tests.Probes;

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
    public void Read_NonNegativeInteger_ReadsAUint()
    {
        var value = NotationReader.Read("43261596", typeof(uint));

        Assert.Equal(43261596u, value);
    }

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
    public void CanRead_RectangularArray_IsTrue() => Assert.True(NotationReader.CanRead(typeof(int[,])));

    [Fact]
    public void CanRead_ArrayOfRankThree_IsFalse() => Assert.False(NotationReader.CanRead(typeof(int[,,])));

    [Fact]
    public void Read_RowsOfOneLength_FillARectangularArray()
    {
        var grid = (char[,]?)NotationReader.Read("[[\"(\",\"(\"],[\")\",\")\"],[\"(\",\")\"]]", typeof(char[,]));

        Assert.Equal(new[,] { { '(', '(' }, { ')', ')' }, { '(', ')' } }, grid);
    }

    [Fact]
    public void Read_NoRows_IsAnEmptyRectangularArray()
    {
        var grid = NotationReader.Read("[]", typeof(int[,]));

        Assert.Equal(new int[0, 0], grid);
    }

    [Fact]
    public void Read_RowsOfDifferentLengths_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() => NotationReader.Read("[[1,2],[3]]", typeof(int[,])));

    [Fact]
    public void Read_RoleTypeOfOneValue_ReadsTheValueIntoIt()
    {
        var role = NotationReader.Read("\"leet\"", typeof(ProbeRoleText));

        Assert.Equal(new ProbeRoleText("leet"), role);
    }

    [Fact]
    public void Read_ArrayOfRoleTypes_ReadsEachItem()
    {
        var items = NotationReader.Read("[\"a\",\"b\"]", typeof(ProbeRoleText[]));

        Assert.Equal<ProbeRoleText>([new("a"), new("b")], Assert.IsType<ProbeRoleText[]>(items));
    }

    [Fact]
    public void CanRead_RecordOfTwoValues_IsTrue() => Assert.True(NotationReader.CanRead(typeof(ProbeEndpoints)));

    [Fact]
    public void Read_RecordOfTwoValuesFromAnArray_ReadsEachValueIntoIt()
    {
        var endpoints = NotationReader.Read("[3,7]", typeof(ProbeEndpoints));

        Assert.Equal(new ProbeEndpoints(3, 7), endpoints);
    }

    // A type and how many consecutive inputs it can stand for.
    public static TheoryData<Type, int> Widths => new()
    {
        { typeof((int, int, int)), 3 },
        { typeof(ProbeEndpoints), 2 },
        { typeof(ProbeRoleText), 1 },
        { typeof(int[]), 1 },
    };

    [Theory]
    [MemberData(nameof(Widths))]
    public void WidthOf_Type_CountsTheValuesItIsBuiltFrom(Type type, int width) => Assert.Equal(width, NotationReader.WidthOf(type));

    [Fact]
    public void ReadSpread_TwoTextsAsARecordOfTwo_BuildsItFromBoth()
    {
        var endpoints = NotationReader.ReadSpread(["3", "7"], typeof(ProbeEndpoints));

        Assert.Equal(new ProbeEndpoints(3, 7), endpoints);
    }

    [Fact]
    public void ReadSpread_ThreeTextsAsATuple_BuildsItFromAll()
    {
        var query = NotationReader.ReadSpread(["0", "4", "9"], typeof((int Source, int Destination, int Target)));

        Assert.Equal((0, 4, 9), query);
    }

    [Fact]
    public void Read_NullableValues_ReadLeetCodesNullAsNoValue()
    {
        var node = NotationReader.Read("[7,null]", typeof(int?[]));

        Assert.Equal(new int?[] { 7, null }, node);
    }

    [Fact]
    public void CanRead_NullableOfAReadableType_IsTrue() => Assert.True(NotationReader.CanRead(typeof(int?[][])));
}
