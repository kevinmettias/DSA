using DSAExperimentation.LeetCode.SubstringMatchingPattern;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubstringMatchingPattern;

// Harness only. Both strategies are SubstringMatchingPatternSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class SubstringMatchingPatternTests
{
    public static TheoryData<MatchExample> Examples =>
        new()
        {
            { new MatchExample(Subject: "leetcode", Pattern: "ee*e", Expected: true) },
            { new MatchExample(Subject: "car", Pattern: "c*v", Expected: false) },
            { new MatchExample(Subject: "luck", Pattern: "u*", Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasMatchByBruteForce_LeetCodeExamples_ReturnsWhetherPatternMatches(MatchExample example)
    {
        var actual = SubstringMatchingPatternSolution.HasMatchByBruteForce(
            new SubjectText(example.Subject),
            new WildcardPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasMatchByPrefixFunctionSearch_LeetCodeExamples_ReturnsWhetherPatternMatches(MatchExample example)
    {
        var actual = SubstringMatchingPatternSolution.HasMatchByPrefixFunctionSearch(
            new SubjectText(example.Subject),
            new WildcardPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the subject, the pattern with its single '*' wildcard, and
    // whether the subject contains a match. Both values are `string`, so the row names
    // which role each plays rather than leaving two positions the compiler would
    // accept either way round.
    public readonly record struct MatchExample(string Subject, string Pattern, bool Expected);
}
