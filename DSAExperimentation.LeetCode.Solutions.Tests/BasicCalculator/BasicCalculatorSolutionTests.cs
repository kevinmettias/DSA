using DSAExperimentation.LeetCode.Solutions.BasicCalculator;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BasicCalculator;

// Harness only. Both strategies are BasicCalculatorSolution's - this file just
// pins them to a parenthesized difference subtracted from a number, whose minus
// sign flips the sign of every term inside the parentheses. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class BasicCalculatorSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "2-(5-6)", 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CalculateByStackScan_Cases_ReturnsEvaluatedResult(string expression, int expected) =>
        Assert.Equal(expected, BasicCalculatorSolution.CalculateByStackScan(expression));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CalculateByRecursiveDescent_Cases_ReturnsEvaluatedResult(string expression, int expected) =>
        Assert.Equal(expected, BasicCalculatorSolution.CalculateByRecursiveDescent(expression));
}
