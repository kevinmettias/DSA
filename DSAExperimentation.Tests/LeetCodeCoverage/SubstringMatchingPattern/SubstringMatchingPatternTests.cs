using DSAExperimentation.LeetCode.SubstringMatchingPattern;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubstringMatchingPattern;

// Harness only. Both strategies are SubstringMatchingPatternSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class SubstringMatchingPatternTests
{
    public static TheoryData<string, string, bool> Examples =>
        new()
        {
            { "leetcode", "ee*e", true },
            { "car", "c*v", false },
            { "luck", "u*", true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasMatchByBruteForce_LeetCodeExamples_ReturnsWhetherPatternMatches(string s, string p, bool expected) =>
        Assert.Equal(
            expected,
            SubstringMatchingPatternSolution.HasMatchByBruteForce(new SubjectText(s), new WildcardPattern(p)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasMatchByPrefixFunctionSearch_LeetCodeExamples_ReturnsWhetherPatternMatches(string s, string p, bool expected) =>
        Assert.Equal(
            expected,
            SubstringMatchingPatternSolution.HasMatchByPrefixFunctionSearch(
                new SubjectText(s), new WildcardPattern(p)));
}
