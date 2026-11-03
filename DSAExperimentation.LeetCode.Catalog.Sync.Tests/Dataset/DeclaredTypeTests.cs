using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests.Dataset;

public sealed partial class DeclaredTypeTests
{
    // A value in LeetCode's notation and the type its parameter declares.
    public static TheoryData<string, string> OfTheDeclaredType =>
        new()
        {
            { "7", "integer" },
            { "8589934592", "long" },
            { "2.5", "double" },
            { "3", "double" },
            { "true", "boolean" },
            { "\"abc\"", "string" },
            { "\"a\"", "character" },
            { "[1,2,3]", "integer[]" },
            { "[[1,2],[]]", "integer[][]" },
            { "[\"a\",\"b\"]", "list<string>" },
            { "[[\"a\"],[\"b\",\"c\"]]", "list<list<string>>" },
            { "[1,null,2]", "TreeNode" },
            { "[]", "TreeNode" },
            { "[1,2]", "ListNode" },
            { "[[1],2]", "list<NestedInteger>" },
        };

    public static TheoryData<string, string> NotOfTheDeclaredType =>
        new()
        {
            { "1.5", "integer" },
            { "2147483648", "integer" },
            { "\"7\"", "integer" },
            { "null", "integer" },
            { "1", "boolean" },
            { "1", "string" },
            { "\"ab\"", "character" },
            { "[1,2.5]", "integer[]" },
            { "[[1],[[]]]", "integer[][]" },
            { "[\"1\",-1]", "string[]" },
            { "7", "integer[]" },
            { "[1,null,1.5]", "TreeNode" },
            { "[1,null]", "ListNode" },
        };

    [Theory]
    [MemberData(nameof(OfTheDeclaredType))]
    public void IsOf_ValueOfTheDeclaredType_IsTrue(string notation, string type)
    {
        var value = JsonNode.Parse(notation);

        var isOfType = DeclaredType.IsOf(value, type);

        Assert.True(isOfType);
    }

    [Theory]
    [MemberData(nameof(NotOfTheDeclaredType))]
    public void IsOf_ValueThatBreaksTheDeclaredType_IsFalse(string notation, string type)
    {
        var value = JsonNode.Parse(notation);

        var isOfType = DeclaredType.IsOf(value, type);

        Assert.False(isOfType);
    }
}
