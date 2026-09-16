using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ParseLispExpressionBenchmarks (ARCHITECTURE 17.9): its two arms are
// ParseLispExpressionSolution's, competing environment strategies for the same interpreter - a
// full copy of the enclosing scope per "let" against chaining each "let"'s bindings onto the
// scope below it - so a harness whose arms disagree is timing two different problems. Setup
// builds the nested-let expression from Length alone, so the same Length must rebuild the same
// expression.
public sealed partial class ParseLispExpressionBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().CopyEnvironmentPerLet(), BuildHarness().CopyEnvironmentPerLet());

    [Fact]
    public void CopyEnvironmentPerLet_NestedLetExpression_AgreesWithScopeChain()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ScopeChain(), harness.CopyEnvironmentPerLet());
    }

    [Fact]
    public void ScopeChain_NestedLetExpression_AgreesWithCopyEnvironmentPerLet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CopyEnvironmentPerLet(), harness.ScopeChain());
    }

    private static ParseLispExpressionBenchmarks BuildHarness()
    {
        var harness = new ParseLispExpressionBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
