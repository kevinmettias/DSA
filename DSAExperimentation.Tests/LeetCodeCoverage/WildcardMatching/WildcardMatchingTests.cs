using DSAExperimentation.LeetCode.WildcardMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WildcardMatching;

// Harness only: both strategies live in WildcardMatchingSolution - the textbook
// greedy two-pointer scan, and a memoized top-down recurrence over this repo's own
// Memoizer.
public sealed class WildcardMatchingTests
{
    public static TheoryData<MatchExample> Examples =>
        new()
        {
            { new MatchExample(Text: "aa", Pattern: "a", Expected: false) },
            { new MatchExample(Text: "aa", Pattern: "*", Expected: true) },
            { new MatchExample(Text: "cb", Pattern: "?a", Expected: false) },
            { new MatchExample(Text: "adceb", Pattern: "*a*b", Expected: true) },
            { new MatchExample(Text: "", Pattern: "", Expected: true) },
            { new MatchExample(Text: "", Pattern: "*", Expected: true) },
            { new MatchExample(Text: "", Pattern: "a", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByGreedyTwoPointer_LeetCodeExamples_ReturnsExpected(MatchExample example)
    {
        var actual = WildcardMatchingSolution.IsMatchByGreedyTwoPointer(
            new WildcardMatchingSolution.MatchedText(example.Text),
            new WildcardMatchingSolution.WildcardPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByMemoizedDp_LeetCodeExamples_ReturnsExpected(MatchExample example)
    {
        var actual = WildcardMatchingSolution.IsMatchByMemoizedDp(
            new WildcardMatchingSolution.MatchedText(example.Text),
            new WildcardMatchingSolution.WildcardPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the subject text, the pattern with its '*' and '?'
    // wildcards, and whether the pattern matches the whole subject. Text and pattern
    // are both `string`, so the row names the roles rather than leaving two adjacent
    // positions a transposition would silently reverse.
    public readonly record struct MatchExample(string Text, string Pattern, bool Expected);
}
