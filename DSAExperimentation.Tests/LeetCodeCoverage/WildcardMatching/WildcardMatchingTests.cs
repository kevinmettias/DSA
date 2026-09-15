using DSAExperimentation.LeetCode.WildcardMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WildcardMatching;

// Harness only: both strategies live in WildcardMatchingSolution - the textbook
// greedy two-pointer scan, and a memoized top-down recurrence over this repo's own
// Memoizer.
public sealed class WildcardMatchingTests
{
    public static TheoryData<string, string, bool> Examples =>
        new()
        {
            { "aa", "a", false },
            { "aa", "*", true },
            { "cb", "?a", false },
            { "adceb", "*a*b", true },
            { "", "", true },
            { "", "*", true },
            { "", "a", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByGreedyTwoPointer_LeetCodeExamples_ReturnsExpected(
        string text, string pattern, bool expected) =>
        Assert.Equal(expected, WildcardMatchingSolution.IsMatchByGreedyTwoPointer(
            new WildcardMatchingSolution.MatchedText(text),
            new WildcardMatchingSolution.WildcardPattern(pattern)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByMemoizedDp_LeetCodeExamples_ReturnsExpected(
        string text, string pattern, bool expected) =>
        Assert.Equal(expected, WildcardMatchingSolution.IsMatchByMemoizedDp(
            new WildcardMatchingSolution.MatchedText(text),
            new WildcardMatchingSolution.WildcardPattern(pattern)));
}
