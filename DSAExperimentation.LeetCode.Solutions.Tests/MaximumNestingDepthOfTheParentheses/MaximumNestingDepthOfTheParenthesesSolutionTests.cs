using DSAExperimentation.LeetCode.Solutions.MaximumNestingDepthOfTheParentheses;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNestingDepthOfTheParentheses;

// Harness only. Both strategies are MaximumNestingDepthOfTheParenthesesSolution's -
// this file just pins them to three expressions without parentheses, whose answer
// is zero. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MaximumNestingDepthOfTheParenthesesSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "1", 0 },
            { "8", 0 },
            { "1+2*3/4-5", 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxDepthByRunningCounter_Cases_ReturnsDeepestNestingLevel(
        string expression, int expected) =>
        Assert.Equal(expected, MaximumNestingDepthOfTheParenthesesSolution.MaxDepthByRunningCounter(expression));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxDepthByOpenerStack_Cases_ReturnsDeepestNestingLevel(
        string expression, int expected) =>
        Assert.Equal(expected, MaximumNestingDepthOfTheParenthesesSolution.MaxDepthByOpenerStack(expression));
}
