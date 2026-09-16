using DSAExperimentation.LeetCode.FindTheOccurrenceOfFirstAlmostEqualSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheOccurrenceOfFirstAlmostEqualSubstring;

// Harness only: both strategies live in
// FindTheOccurrenceOfFirstAlmostEqualSubstringSolution. This file just pins
// them to #3303's published examples, including the no-match case.
public sealed class FindTheOccurrenceOfFirstAlmostEqualSubstringTests
{
    public static TheoryData<AlmostEqualCase> Examples =>
        new()
        {
            { new AlmostEqualCase(Text: "abcdefg", Pattern: "bcdffg", Expected: 1) },
            { new AlmostEqualCase(Text: "ababbababa", Pattern: "bacaba", Expected: 4) },
            { new AlmostEqualCase(Text: "abcd", Pattern: "dba", Expected: -1) },
            { new AlmostEqualCase(Text: "dde", Pattern: "d", Expected: 0) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfFirstAlmostEqualSubstringByBruteForce_LeetCodeExamples_ReturnsFirstQualifyingIndex(
        AlmostEqualCase example)
    {
        var actual =
            FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.IndexOfFirstAlmostEqualSubstringByBruteForce(
                new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.SearchedText(example.Text),
                new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.MatchPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IndexOfFirstAlmostEqualSubstringByZFunction_LeetCodeExamples_ReturnsFirstQualifyingIndex(
        AlmostEqualCase example)
    {
        var actual =
            FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.IndexOfFirstAlmostEqualSubstringByZFunction(
                new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.SearchedText(example.Text),
                new FindTheOccurrenceOfFirstAlmostEqualSubstringSolution.MatchPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the text searched, the pattern sought in it, and the first
    // index where the pattern matches with at most one differing character (-1 when no such
    // index exists). The text and the pattern are named fields rather than two adjacent
    // `string` parameters, so a row is written `new AlmostEqualCase(Text: ..., Pattern:
    // ...)` and a text/pattern swap has to be typed out by name instead of falling out of a
    // position the compiler would have accepted either way. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct AlmostEqualCase(string Text, string Pattern, int Expected);
}
