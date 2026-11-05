using System.Reflection;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Harness.Tests.Probes;

namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class CatalogCallsTests
{
    private static readonly MethodInfo Factory =
        typeof(ScriptProbeSolution).GetMethod(nameof(ScriptProbeSolution.CreateByDictionary))
            ?? throw new InvalidOperationException("The probe has no factory.");

    // put returns nothing; get answers through the probe's Calls type, -1 for the key never put.
    [Fact]
    public void AnswerTo_AnyScript_WritesNullForTheConstructorAndEachCallReturningNothing()
    {
        var calls = CatalogCallBinder.Bind(Factory, typeof(ScriptProbeCalls), LeetCodeCatalog.Question("lru-cache").Signature.Methods);
        var arguments = JsonNode.Parse("[[2],[1,7],[1],[3]]")?.AsArray() ?? [];

        Assert.Equal("[null,null,7,-1]", calls.AnswerTo(["LRUCache", "put", "get", "get"], arguments).ToJsonString());
    }

    [Fact]
    public void ArgumentTextOf_ArgumentLeetCodeListsAsNull_IsNullsNotation()
    {
        var text = CatalogCalls.ArgumentTextOf(JsonNode.Parse("[null]"), 0);

        Assert.Equal("null", text);
    }

    [Fact]
    public void ArgumentTextOf_AnyOtherArgument_IsItsNotation()
    {
        var text = CatalogCalls.ArgumentTextOf(JsonNode.Parse("[3,[1,2]]"), 1);

        Assert.Equal("[1,2]", text);
    }
}
