using DSAExperimentation.LeetCode.ParsingABooleanExpression;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ParsingABooleanExpression;

// Harness only: both strategies live in ParsingABooleanExpressionSolution and are
// asserted against the same expressions - the leaf cases, each operator on its own,
// and the nested expression LeetCode itself publishes.
public sealed class ParsingABooleanExpressionTests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "t", true },
            { "f", false },
            { "!(f)", true },
            { "!(t)", false },
            { "&(t,f)", false },
            { "&(t,t,t)", true },
            { "|(t,f)", true },
            { "|(f,f,f)", false },
            { "|(&(t,f,t),!(t))", false },
            { "&(|(f),t)", false },
            { "|(!(&(t,f)),f)", true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ParseBoolExprByRecursiveDescent_LeetCodeExamples_EvaluatesToExpectedBoolean(
        string expression,
        bool expected) =>
        Assert.Equal(expected, ParsingABooleanExpressionSolution.ParseBoolExprByRecursiveDescent(expression));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ParseBoolExprByParserStack_LeetCodeExamples_EvaluatesToExpectedBoolean(
        string expression,
        bool expected) =>
        Assert.Equal(expected, ParsingABooleanExpressionSolution.ParseBoolExprByParserStack(expression));
}
