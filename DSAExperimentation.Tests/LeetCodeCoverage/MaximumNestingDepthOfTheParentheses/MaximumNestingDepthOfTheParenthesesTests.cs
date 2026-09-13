using DSAExperimentation.LeetCode.MaximumNestingDepthOfTheParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNestingDepthOfTheParentheses;

// Harness only. Both strategies are MaximumNestingDepthOfTheParenthesesSolution's -
// this file just pins them to LeetCode's published examples, the sibling-groups case
// where the deepest group is not the first, and the two no-parentheses expressions
// whose answer is zero.
public sealed class MaximumNestingDepthOfTheParenthesesTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "(1+(2*3)+((8)/4))+1", 3 },
            { "(1)+((2))+(((3)))", 3 },
            { "()(())((()()))", 3 },
            { "1", 0 },
            { "8", 0 },
            { "1+2*3/4-5", 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDepthByRunningCounter_LeetCodeExamples_ReturnsDeepestNestingLevel(
        string s, int expected) =>
        Assert.Equal(expected, MaximumNestingDepthOfTheParenthesesSolution.MaxDepthByRunningCounter(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxDepthByOpenerStack_LeetCodeExamples_ReturnsDeepestNestingLevel(
        string s, int expected) =>
        Assert.Equal(expected, MaximumNestingDepthOfTheParenthesesSolution.MaxDepthByOpenerStack(s));
}
