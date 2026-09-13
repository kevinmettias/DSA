using DSAExperimentation.LeetCode.MinimumAddToMakeParenthesesValid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAddToMakeParenthesesValid;

// Harness only: both strategies live in MinimumAddToMakeParenthesesValidSolution
// and are asserted against the same examples, including the closer-before-opener
// case where neither half of the count can be inferred from the other.
public sealed class MinimumAddToMakeParenthesesValidTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "())", 1 },
            { "(((", 3 },
            { "()", 0 },
            { "()))((", 4 },
            { ")(", 2 },
            { "()()", 0 },
            { "(()())", 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAddToMakeValidByRunningCounter_LeetCodeExamples_ReturnsInsertionsNeeded(
        string s,
        int expected) =>
        Assert.Equal(expected, MinimumAddToMakeParenthesesValidSolution.MinAddToMakeValidByRunningCounter(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinAddToMakeValidByOpenerStack_LeetCodeExamples_ReturnsInsertionsNeeded(
        string s,
        int expected) =>
        Assert.Equal(expected, MinimumAddToMakeParenthesesValidSolution.MinAddToMakeValidByOpenerStack(s));
}
