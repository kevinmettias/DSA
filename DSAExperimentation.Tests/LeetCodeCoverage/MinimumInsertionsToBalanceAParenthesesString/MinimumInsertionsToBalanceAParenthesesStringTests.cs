using DSAExperimentation.LeetCode.MinimumInsertionsToBalanceAParenthesesString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumInsertionsToBalanceAParenthesesString;

// Harness only: both strategies live in MinimumInsertionsToBalanceAParenthesesStringSolution
// and are asserted against the same examples, including the all-openers and
// all-closers extremes that separate "insert a closer" from "insert an opener".
public sealed class MinimumInsertionsToBalanceAParenthesesStringTests
{
    public static TheoryData<string, int> Examples =>
        new()
        {
            { "(()))", 1 },
            { "())", 0 },
            { "))())(", 3 },
            { "((((((", 12 },
            { ")))))))", 5 },
            { "()", 1 },
            { "((", 4 },
            { "()))", 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinInsertionsByRunningCounter_LeetCodeExamples_ReturnsMinimumInsertions(string text, int expected) =>
        Assert.Equal(
            expected,
            MinimumInsertionsToBalanceAParenthesesStringSolution.MinInsertionsByRunningCounter(text));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinInsertionsByOpenerStack_LeetCodeExamples_ReturnsMinimumInsertions(string text, int expected) =>
        Assert.Equal(
            expected,
            MinimumInsertionsToBalanceAParenthesesStringSolution.MinInsertionsByOpenerStack(text));
}
