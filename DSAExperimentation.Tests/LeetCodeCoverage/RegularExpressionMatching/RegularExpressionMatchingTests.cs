using DSAExperimentation.LeetCode.RegularExpressionMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RegularExpressionMatching;

// Harness only: both strategies live in RegularExpressionMatchingSolution and are
// asserted against the same examples.
public sealed partial class RegularExpressionMatchingTests
{
    public static TheoryData<MatchExample> Examples =>
        new()
        {
            { new MatchExample(Text: "aa", Pattern: "a", Expected: false) },
            { new MatchExample(Text: "aa", Pattern: "a*", Expected: true) },
            { new MatchExample(Text: "ab", Pattern: ".*", Expected: true) },
            { new MatchExample(Text: "aab", Pattern: "c*a*b", Expected: true) },
            { new MatchExample(Text: "mississippi", Pattern: "mis*is*p*.", Expected: false) },
            { new MatchExample(Text: "", Pattern: "c*", Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByRecursion_LeetCodeExamples_ReturnsExpectedResult(MatchExample example)
    {
        var actual = RegularExpressionMatchingSolution.IsMatchByRecursion(
            new RegularExpressionMatchingSolution.SubjectText(example.Text),
            new RegularExpressionMatchingSolution.RegexPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByMemoization_LeetCodeExamples_ReturnsExpectedResult(MatchExample example)
    {
        var actual = RegularExpressionMatchingSolution.IsMatchByMemoization(
            new RegularExpressionMatchingSolution.SubjectText(example.Text),
            new RegularExpressionMatchingSolution.RegexPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the subject text, the pattern, and whether the pattern
    // matches the subject as a whole. Text and pattern are both `string`; naming
    // them in the row is what keeps a transposition from silently asking whether the
    // pattern occurs inside the text instead.
    public readonly record struct MatchExample(string Text, string Pattern, bool Expected);
}
