using DSAExperimentation.LeetCode.ParsingABooleanExpression;

namespace DSAExperimentation.LeetCode.Tests.ParsingABooleanExpression;

// Harness only: both strategies live in ParsingABooleanExpressionSolution and are
// asserted against the same expressions - LeetCode's three published examples, the
// leaf cases, each operator on its own, and three nested expressions of this file's
// own.
public sealed partial class ParsingABooleanExpressionSolutionTests
{
    public static TheoryData<ExpressionExample> Examples =>
        new()
        {
            // LeetCode examples 1-3.
            { new ExpressionExample(Expression: "&(|(f))", Expected: false) },
            { new ExpressionExample(Expression: "|(f,f,f,t)", Expected: true) },
            { new ExpressionExample(Expression: "!(&(f,t))", Expected: true) },

            // The leaves and each operator alone. Then three nestings, evaluated inside
            // out: &(t,f,t) = f and !(t) = f, so their OR is f; |(f) = f, so its AND with
            // t is f; &(t,f) = f, its NOT is t, so the OR with f is t.
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

    // One example: the expression and the value it evaluates to. The answer
    // is the datum under test, so the row names it rather than leaving a bare `bool`
    // beside the expression - a bare
    // `IsBoolExprTrueByRecursiveDescent("t", true)` does not say whether that `true`
    // is the expected result or a parse flag.
    public readonly record struct ExpressionExample(string Expression, bool Expected);
}
