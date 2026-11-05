using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Harness.Tests.Probes;

namespace DSAExperimentation.LeetCode.Harness.Tests;

// Over LRU Cache's one example: ["LRUCache","put","put","get","put","get","put","get","get","get"] with
// [[2],[1,1],[2,2],[1],[3,3],[2],[4,4],[1],[3],[4]]. Its signature lists get, then put.
public sealed partial class CatalogScriptsTests
{
    private const int Get = 0;

    private static CatalogScripts Scripts =>
        new(CatalogColumns.ForScripts(typeof(ScriptProbeSolution)), [.. LeetCodeCatalog.Question("lru-cache").Signature.Methods.Select(method => method.Name)]);

    [Fact]
    public void Calls_ExampleScript_MarksTheConstructorThenEachCallsMethod() =>
        Assert.Equal([CatalogScripts.Constructor, 1, 1, 0, 1, 0, 1, 0, 0, 0], Scripts.Calls[0]);

    [Fact]
    public void ArgumentColumn_OneMethod_HoldsItsCallsArgumentsAndTheDefaultElsewhere() =>
        Assert.Equal([0, 0, 0, 1, 0, 2, 0, 1, 3, 4], Scripts.ArgumentColumn<int>(Get, 0)[0]);

    [Fact]
    public void ConstructorColumn_AnyInput_HoldsEachCasesConstructorArgument() =>
        Assert.Equal([2], Scripts.ConstructorColumn<int>(0));

    [Fact]
    public void AnswerBuffer_AnyMethod_HoldsASlotForEveryCall() =>
        Assert.Equal(Scripts.Calls[0].Length, Scripts.AnswerBuffer<int>()[0].Length);

    [Fact]
    public void AnswersFrom_EachMethodsBuffer_AnswersEveryCase() =>
        Assert.Equal(1, Scripts.AnswersFrom([Scripts.AnswerBuffer<int>(), null]).Count);
    [Fact]
    public void New_MethodsOmittingOneAScriptCalls_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() => new CatalogScripts(CatalogColumns.ForScripts(typeof(ScriptProbeSolution)), ["get"]));
}
