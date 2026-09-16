using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ParsingABooleanExpressionBenchmarks (ARCHITECTURE 17.9): its two arms are
// ParsingABooleanExpressionSolution's, competing evaluators for the same expression - the
// recursive descent against an explicit operator stack - so a harness whose arms disagree is
// timing two different problems. Depth controls only how large a generated expression is, so the
// same Depth must rebuild the same expression and therefore the same verdict.
public sealed partial class ParsingABooleanExpressionBenchmarksTests
{
    private const int SmallestDepth = 8;

    [Fact]
    public void Setup_SameDepth_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsBoolExprTrueByRecursiveDescent(),
            BuildHarness().IsBoolExprTrueByRecursiveDescent());

    [Fact]
    public void IsBoolExprTrueByRecursiveDescent_SeededExpression_AgreesWithIsBoolExprTrueByParserStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsBoolExprTrueByParserStack(), harness.IsBoolExprTrueByRecursiveDescent());
    }

    [Fact]
    public void IsBoolExprTrueByParserStack_SeededExpression_AgreesWithIsBoolExprTrueByRecursiveDescent()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsBoolExprTrueByRecursiveDescent(), harness.IsBoolExprTrueByParserStack());
    }

    private static ParsingABooleanExpressionBenchmarks BuildHarness()
    {
        var harness = new ParsingABooleanExpressionBenchmarks { Depth = SmallestDepth };
        harness.Setup();

        return harness;
    }
}
