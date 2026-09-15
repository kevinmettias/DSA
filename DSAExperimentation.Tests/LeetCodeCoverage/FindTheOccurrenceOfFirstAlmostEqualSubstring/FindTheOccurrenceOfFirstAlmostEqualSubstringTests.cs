using DSAExperimentation.LeetCode.FindTheOccurrenceOfFirstAlmostEqualSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheOccurrenceOfFirstAlmostEqualSubstring;

// Harness only: both strategies live in
// FindTheOccurrenceOfFirstAlmostEqualSubstringSolution. This file just pins
// them to #3303's published examples, including the no-match case.
public sealed class FindTheOccurrenceOfFirstAlmostEqualSubstringTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "abcdefg", "bcdffg", 1 },
            { "ababbababa", "bacaba", 4 },
            { "abcd", "dba", -1 },
            { "dde", "d", 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfFirstAlmostEqualSubstringByBruteForce_LeetCodeExamples_ReturnsFirstQualifyingIndex(
        string s, string pattern, int expected) =>
        Assert.Equal(
            expected,
            FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.IndexOfFirstAlmostEqualSubstringByBruteForce(
                new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.SearchedText(s),
                new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.MatchPattern(pattern)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfFirstAlmostEqualSubstringByZFunction_LeetCodeExamples_ReturnsFirstQualifyingIndex(
        string s, string pattern, int expected) =>
        Assert.Equal(
            expected,
            FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.IndexOfFirstAlmostEqualSubstringByZFunction(
                new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.SearchedText(s),
                new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.MatchPattern(pattern)));
}
