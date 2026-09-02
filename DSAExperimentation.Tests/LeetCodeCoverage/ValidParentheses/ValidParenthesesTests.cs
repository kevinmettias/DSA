using DSAExperimentation.LeetCode.ValidParentheses;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidParentheses;

// Harness only. The bracket-matching walk is ValidParenthesesSolution's - this
// file just pins it to LeetCode's published examples.
public sealed class ValidParenthesesTests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            { "([{}])", true },
            { "(]", false },
            { "((", false },
            { "()", true },
            { "()[]{}", true },
            { "([)]", false },
            { "", true },
            { ")", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsValidByBracketStack_LeetCodeExamples_ReturnsWhetherProperlyNested(
        string brackets, bool expected) =>
        Assert.Equal(expected, ValidParenthesesSolution.IsValidByBracketStack(brackets));
}
