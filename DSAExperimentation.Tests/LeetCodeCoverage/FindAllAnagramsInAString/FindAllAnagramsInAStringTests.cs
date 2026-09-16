using DSAExperimentation.LeetCode.FindAllAnagramsInAString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindAllAnagramsInAString;

// Harness only. Both strategies are FindAllAnagramsInAStringSolution's - this file
// just pins them to LeetCode's published examples, including the pattern-longer-
// than-string case both strategies must short-circuit on.
public sealed partial class FindAllAnagramsInAStringTests
{
    public static TheoryData<AnagramCase> Examples =>
        new()
        {
            { new AnagramCase(Text: "cbaebabacd", Pattern: "abc", Expected: [0, 6]) },
            { new AnagramCase(Text: "abab", Pattern: "ab", Expected: [0, 1, 2]) },
            { new AnagramCase(Text: "a", Pattern: "aa", Expected: []) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAnagramIndicesByBruteForceRebuild_LeetCodeExamples_ReturnsEveryAnagramStart(
        AnagramCase example)
    {
        var actual = FindAllAnagramsInAStringSolution.FindAnagramIndicesByBruteForceRebuild(
            new ScannedText(example.Text), new AnagramPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAnagramIndicesBySlidingWindow_LeetCodeExamples_ReturnsEveryAnagramStart(
        AnagramCase example)
    {
        var actual = FindAllAnagramsInAStringSolution.FindAnagramIndicesBySlidingWindow(
            new ScannedText(example.Text), new AnagramPattern(example.Pattern));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the scanned text, the anagram pattern sought in it, and every
    // start index where one occurs. The text and the pattern are named fields rather than
    // two adjacent `string` parameters, so a row is written `new AnagramCase(Text: ...,
    // Pattern: ...)` and a text/pattern swap has to be typed out by name instead of falling
    // out of a position the compiler would have accepted either way. Nested because it is
    // only ever used inside this test class - it is this harness's own vocabulary, not a
    // type another file would import.
    public readonly record struct AnagramCase(string Text, string Pattern, int[] Expected);
}
