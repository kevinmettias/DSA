using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EvaluateReversePolishNotationBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm, so there is no second strategy to reconcile and no [GlobalSetup] and no [Params] to
// rebuild. The arm is handed LeetCode 150's own third example - a fully nested expression using
// every operator - so the assertion is against that example's published value, which the harness's
// fixed token array makes decisive rather than a restatement of whatever the arm happens to return.
public sealed partial class EvaluateReversePolishNotationBenchmarksTests
{
    // LeetCode 150's third example evaluates to 22.
    private const int ExpectedEvaluation = 22;

    [Fact]
    public void OperandStack_NestedOperatorExpression_ReturnsLeetCodeExampleValue() =>
        Assert.Equal(ExpectedEvaluation, new EvaluateReversePolishNotationBenchmarks().OperandStack());
}
