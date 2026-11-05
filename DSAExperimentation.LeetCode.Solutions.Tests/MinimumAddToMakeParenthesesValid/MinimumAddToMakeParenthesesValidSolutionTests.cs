using DSAExperimentation.LeetCode.Solutions.MinimumAddToMakeParenthesesValid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumAddToMakeParenthesesValid;

// Harness only: both strategies live in MinimumAddToMakeParenthesesValidSolution
// and are asserted against the same cases, among them the closer-before-opener
// case where neither half of the count can be inferred from the other.
public sealed partial class MinimumAddToMakeParenthesesValidSolutionTests
{
    public static TheoryData<string, int> Cases =>
        new()
        {
            { ")(", 2 },
            { "()()", 0 },
            { "(()())", 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinAddToMakeValidByRunningCounter_Cases_ReturnsInsertionsNeeded(
        string text,
        int expected) =>
        Assert.Equal(expected, MinimumAddToMakeParenthesesValidSolution.MinAddToMakeValidByRunningCounter(text));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinAddToMakeValidByOpenerStack_Cases_ReturnsInsertionsNeeded(
        string text,
        int expected) =>
        Assert.Equal(expected, MinimumAddToMakeParenthesesValidSolution.MinAddToMakeValidByOpenerStack(text));
}
