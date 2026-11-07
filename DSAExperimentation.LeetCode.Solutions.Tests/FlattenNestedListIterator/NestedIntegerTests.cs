using System.Globalization;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Solutions.FlattenNestedListIterator;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FlattenNestedListIterator;

public sealed partial class NestedIntegerTests
{
    private const string Nested = "[[1,1],2,[]]";

    [Fact]
    public void ListOf_LeetCodeNotation_KeepsEachIntegerAndEachList()
    {
        var elements = NestedInteger.ListOf(ArrayOf(Nested));

        Assert.Equal(Nested, NotationOf(elements));
    }

    [Fact]
    public void ListOf_ElementNeitherIntegerNorList_Throws() =>
        Assert.Throws<ArgumentException>(() => NestedInteger.ListOf(ArrayOf("[1,null]")));

    private static JsonArray ArrayOf(string notation) =>
        JsonNode.Parse(notation) as JsonArray ?? throw new ArgumentException($"{notation} is not a JSON array.", nameof(notation));

    // The nested list written back in LeetCode's notation.
    private static string NotationOf(List<NestedInteger> elements) =>
        $"[{string.Join(",", elements.Select(NotationOf))}]";

    private static string NotationOf(NestedInteger element) =>
        element.IsInteger ? element.Value.ToString(CultureInfo.InvariantCulture) : NotationOf(element.Elements);
}
