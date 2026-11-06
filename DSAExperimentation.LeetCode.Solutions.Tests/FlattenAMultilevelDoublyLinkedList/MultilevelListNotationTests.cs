using DSAExperimentation.LeetCode.Solutions.FlattenAMultilevelDoublyLinkedList;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FlattenAMultilevelDoublyLinkedList;

public sealed partial class MultilevelListNotationTests
{
    private static readonly int?[] FirstExample = [1, 2, 3, 4, 5, 6, null, null, null, 7, 8, 9, 10, null, null, 11, 12];

    [Fact]
    public void From_LeetCodesFirstExample_HangsTheSecondLevelFromTheThirdNode()
    {
        var head = MultilevelListNotation.From(FirstExample);

        Assert.Equal(7, head?.Next?.Next?.Child?.Value);
    }

    [Fact]
    public void From_LeetCodesFirstExample_HangsTheThirdLevelFromTheSecondLevelsSecondNode()
    {
        var head = MultilevelListNotation.From(FirstExample);

        Assert.Equal(11, head?.Next?.Next?.Child?.Next?.Child?.Value);
    }

    [Fact]
    public void ValuesOf_ListWithAChildLeft_IsNone()
    {
        var head = MultilevelListNotation.From([1, 2, null, 3]);

        Assert.Empty(MultilevelListNotation.ValuesOf(head));
    }

    [Fact]
    public void ValuesOf_OneLevel_ReadsItsValuesInOrder()
    {
        var head = MultilevelListNotation.From([1, 2, 3]);

        Assert.Equal([1, 2, 3], MultilevelListNotation.ValuesOf(head));
    }

    [Fact]
    public void From_EmptyNotation_IsNoList() => Assert.Null(MultilevelListNotation.From([]));
}
