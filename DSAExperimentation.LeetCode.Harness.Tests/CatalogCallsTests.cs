using System.Reflection;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Catalog.Questions;
using DSAExperimentation.LeetCode.Harness.Tests.Probes;

namespace DSAExperimentation.LeetCode.Harness.Tests;

public sealed partial class CatalogCallsTests
{
    private static readonly MethodInfo Factory =
        typeof(ScriptProbeSolution).GetMethod(nameof(ScriptProbeSolution.CreateByDictionary))
            ?? throw new InvalidOperationException("The probe has no factory.");

    // LRU Cache's methods, get and put, as its signature lists them.
    private static IReadOnlyList<LeetCodeMethod> Methods => LeetCodeCatalog.Question("lru-cache").Signature.Methods;

    // put returns nothing; get answers through the probe's Calls type, -1 for the key never put.
    [Fact]
    public void AnswerTo_AnyScript_WritesNullForTheConstructorAndEachCallReturningNothing()
    {
        var calls = CatalogCalls.For(Factory, typeof(ScriptProbeCalls), Methods);
        var arguments = JsonNode.Parse("[[2],[1,7],[1],[3]]")?.AsArray() ?? [];

        Assert.Equal("[null,null,7,-1]", calls.AnswerTo(["LRUCache", "put", "get", "get"], arguments).ToJsonString());
    }

    [Fact]
    public void For_CallNeitherTheObjectNorACallsTypeAnswers_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() => CatalogCalls.For(Factory, calls: null, Methods));

    [Fact]
    public void ArgumentTextOf_ArgumentLeetCodeListsAsNull_IsNullsNotation() =>
        Assert.Equal("null", CatalogCalls.ArgumentTextOf(JsonNode.Parse("[null]"), 0));

    [Fact]
    public void ArgumentTextOf_AnyOtherArgument_IsItsNotation() =>
        Assert.Equal("[1,2]", CatalogCalls.ArgumentTextOf(JsonNode.Parse("[3,[1,2]]"), 1));
}
