using DSAExperimentation.LeetCode.ParseLispExpression;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ParseLispExpression;

// Harness only: both strategies live in ParseLispExpressionSolution and are
// asserted against the same examples, including nested shadowing ("let x 2
// (mult x (let x 3 y 4 (add x y))))") and sequential re-binding within one
// "let" ("let x 3 x 2 x").
public sealed class ParseLispExpressionTests
{
    public static TheoryData<string, long> Examples =>
        new()
        {
            { "(let x 2 (mult x (let x 3 y 4 (add x y))))", 14 },
            { "(let x 3 x 2 x)", 2 },
            { "(let x 1 y 2 x (add x y) (add x y))", 5 },
            { "(add 1 2)", 3 },
            { "(mult 3 (add 2 3))", 15 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EvaluateByCopiedScope_LeetCodeExamples_ReturnsExpectedValue(string expression, long expected) =>
        Assert.Equal(expected, ParseLispExpressionSolution.EvaluateByCopiedScope(expression));

    [Theory]
    [MemberData(nameof(Examples))]
    public void EvaluateByScopeChain_LeetCodeExamples_ReturnsExpectedValue(string expression, long expected) =>
        Assert.Equal(expected, ParseLispExpressionSolution.EvaluateByScopeChain(expression));
}
