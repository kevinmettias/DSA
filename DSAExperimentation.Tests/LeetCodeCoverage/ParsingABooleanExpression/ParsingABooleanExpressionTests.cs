using DSAExperimentation.LeetCode.ParsingABooleanExpression;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ParsingABooleanExpression;

// Harness only: both strategies live in ParsingABooleanExpressionSolution and are
// asserted against the same expressions - the leaf cases, each operator on its own,
// and the nested expression LeetCode itself publishes.
public sealed class ParsingABooleanExpressionTests
{
    public static TheoryData<ExpressionExample> Examples =>
        new()
        {
            { new ExpressionExample(Expression: "t", Expected: true) },
            { new ExpressionExample(Expression: "f", Expected: false) },
            { new ExpressionExample(Expression: "!(f)", Expected: true) },
            { new ExpressionExample(Expression: "!(t)", Expected: false) },
            { new ExpressionExample(Expression: "&(t,f)", Expected: false) },
            { new ExpressionExample(Expression: "&(t,t,t)", Expected: true) },
            { new ExpressionExample(Expression: "|(t,f)", Expected: true) },
            { new ExpressionExample(Expression: "|(f,f,f)", Expected: false) },
            { new ExpressionExample(Expression: "|(&(t,f,t),!(t))", Expected: false) },
            { new ExpressionExample(Expression: "&(|(f),t)", Expected: false) },
            { new ExpressionExample(Expression: "|(!(&(t,f)),f)", Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsBoolExprTrueByRecursiveDescent_LeetCodeExamples_EvaluatesToExpectedBoolean(
        ExpressionExample example) =>
        Assert.Equal(
            example.Expected,
            ParsingABooleanExpressionSolution.IsBoolExprTrueByRecursiveDescent(
                example.Expression));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsBoolExprTrueByParserStack_LeetCodeExamples_EvaluatesToExpectedBoolean(
        ExpressionExample example) =>
        Assert.Equal(
            example.Expected,
            ParsingABooleanExpressionSolution.IsBoolExprTrueByParserStack(
                example.Expression));

    // One LeetCode example: the expression and the value it evaluates to. The answer
    // is the datum under test, so the row names it rather than leaving a bare `bool`
    // beside the expression - a bare
    // `IsBoolExprTrueByRecursiveDescent("t", true)` does not say whether that `true`
    // is the expected result or a parse flag.
    public readonly record struct ExpressionExample(string Expression, bool Expected);
}
