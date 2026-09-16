using DSAExperimentation.LeetCode.BasicCalculator;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BasicCalculator;

// Harness only. Both strategies are BasicCalculatorSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class BasicCalculatorTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "1 + 1", 2 },
            { " 2-1 + 2 ", 3 },
            { "(1+(4+5+2)-3)+(6+8)", 23 },
            { "2-(5-6)", 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CalculateByStackScan_LeetCodeExamples_ReturnsEvaluatedResult(string expression, int expected) =>
        Assert.Equal(expected, BasicCalculatorSolution.CalculateByStackScan(expression));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CalculateByRecursiveDescent_LeetCodeExamples_ReturnsEvaluatedResult(string expression, int expected) =>
        Assert.Equal(expected, BasicCalculatorSolution.CalculateByRecursiveDescent(expression));
}
