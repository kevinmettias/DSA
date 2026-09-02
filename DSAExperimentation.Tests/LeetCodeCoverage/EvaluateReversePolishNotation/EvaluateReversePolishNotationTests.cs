using DSAExperimentation.LeetCode.EvaluateReversePolishNotation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EvaluateReversePolishNotation;

// Harness only. The operand-stack evaluation is
// EvaluateReversePolishNotationSolution's; this file pins it to LeetCode's
// published examples.
public sealed class EvaluateReversePolishNotationTests
{
    public static TheoryData<string[], int> Examples =>
        new()
        {
            { ["2", "1", "+", "3", "*"], 9 },
            { ["4", "13", "5", "/", "+"], 6 },
            { ["10", "6", "9", "3", "+", "-11", "*", "/", "*", "17", "+", "5", "+"], 22 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EvalByOperandStack_LeetCodeExamples_ReturnsExpressionValue(string[] tokens, int expected) =>
        Assert.Equal(expected, EvaluateReversePolishNotationSolution.EvalByOperandStack(tokens));
}
