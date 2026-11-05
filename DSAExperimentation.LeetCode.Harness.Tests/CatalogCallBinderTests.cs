using System.Reflection;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Catalog.Questions;
using DSAExperimentation.LeetCode.Harness.Tests.Probes;

namespace DSAExperimentation.LeetCode.Harness.Tests;

// Over LRU Cache's two calls, get and put, on the probe's map: the map answers put itself and get only
// through the probe's Calls type.
public sealed partial class CatalogCallBinderTests
{
    private static readonly MethodInfo Factory =
        typeof(ScriptProbeSolution).GetMethod(nameof(ScriptProbeSolution.CreateByDictionary))
            ?? throw new InvalidOperationException("The probe has no factory.");

    private static IReadOnlyList<LeetCodeMethod> Methods => LeetCodeCatalog.Question("lru-cache").Signature.Methods;

    [Fact]
    public void Bind_CallsTheInstanceAndTheCallsTypeAnswer_BindsEveryCall()
    {
        var calls = CatalogCallBinder.Bind(Factory, typeof(ScriptProbeCalls), Methods);

        Assert.NotNull(calls);
    }

    [Fact]
    public void Bind_CallNeitherTheInstanceNorACallsTypeAnswers_ThrowsInvalidOperationException() =>
        Assert.Throws<InvalidOperationException>(() => CatalogCallBinder.Bind(Factory, calls: null, Methods));
}
