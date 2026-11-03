namespace DSAExperimentation.LeetCode.Catalog.Tests;

public sealed partial class LeetCodeNotationTests
{
    [Fact]
    public void Parse_IntegerArray_ReturnsItsElements()
        => Assert.Equal([2, 7, 11, 15], LeetCodeNotation.Parse<int[]>("[2,7,11,15]"));

    [Fact]
    public void Parse_CharacterGrid_ReadsEachOneLetterStringAsACharacter()
        => Assert.Equal([['1', '0'], ['.', '9']], LeetCodeNotation.Parse<char[][]>("[[\"1\",\"0\"],[\".\",\"9\"]]"));

    [Fact]
    public void Parse_TreeLevelOrder_KeepsNullsForAbsentNodes()
        => Assert.Equal([3, 9, 20, null, null, 15, 7], LeetCodeNotation.Parse<int?[]>("[3,9,20,null,null,15,7]"));

    [Fact]
    public void Parse_NestedListOfStrings_ReturnsEachRow()
    {
        var rows = LeetCodeNotation.Parse<IList<IList<string>>>("[[\"eat\",\"tea\"],[\"bat\"]]");

        Assert.Equal([["eat", "tea"], ["bat"]], rows);
    }

    [Fact]
    public void Parse_BareNull_Throws()
        => Assert.Throws<FormatException>(() => LeetCodeNotation.Parse<int[]>("null"));
}
