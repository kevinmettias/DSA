using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EvaluateReversePolishNotationBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm, so there is no second strategy to reconcile. Setup's expression comes from
// EvaluateReversePolishNotationWorkloads with seeded Random(150), and that fixture returns the value
// it built the expression to have - a value its own tests confirm by replaying the tokens - so the
// assertion is against that construction rather than a restatement of whatever the arm returns.
public sealed partial class EvaluateReversePolishNotationBenchmarksTests
{
    private const int SmallestOperandCount = 50;
    private const int RandomSeed = 150;

    [Fact]
    public void Setup_SameOperandCount_RebuildsTheSameExpression() =>
        Assert.Equal(BuildHarness().OperandStack(), BuildHarness().OperandStack());

    [Fact]
    public void OperandStack_SeededExpression_ReturnsTheValueItWasBuiltToHave() =>
        Assert.Equal(
            EvaluateReversePolishNotationWorkloads.Build(SmallestOperandCount, new Random(RandomSeed)).Value,
            BuildHarness().OperandStack());

    private static EvaluateReversePolishNotationBenchmarks BuildHarness()
    {
        var harness = new EvaluateReversePolishNotationBenchmarks { OperandCount = SmallestOperandCount };
        harness.Setup();

        return harness;
    }
}
