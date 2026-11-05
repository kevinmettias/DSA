using DSAExperimentation.LeetCode.Solutions.MinimumInsertionsToBalanceAParenthesesString;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumInsertionsToBalanceAParenthesesString;

// Harness only: both strategies live in MinimumInsertionsToBalanceAParenthesesStringSolution
// and are asserted against the same cases, among them the all-openers and
// all-closers extremes that separate "insert a closer" from "insert an opener".
public sealed partial class MinimumInsertionsToBalanceAParenthesesStringSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { "((((((", 12 },
            { ")))))))", 5 },
            { "()", 1 },
            { "((", 4 },
            { "()))", 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinInsertionsByRunningCounter_Cases_ReturnsMinimumInsertions(string text, int expected) =>
        Assert.Equal(
            expected,
            MinimumInsertionsToBalanceAParenthesesStringSolution.MinInsertionsByRunningCounter(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinInsertionsByOpenerStack_Cases_ReturnsMinimumInsertions(string text, int expected) =>
        Assert.Equal(
            expected,
            MinimumInsertionsToBalanceAParenthesesStringSolution.MinInsertionsByOpenerStack(text));
}
