using DSAExperimentation.LeetCode.MinimumWindowSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumWindowSubstring;

// Harness only. Both strategies are MinimumWindowSubstringSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class MinimumWindowSubstringTests
{
    public static TheoryData<WindowExample> Examples =>
        new()
        {
            { new WindowExample(Text: "ADOBECODEBANC", Required: "ABC", Expected: "BANC") },
            { new WindowExample(Text: "a", Required: "a", Expected: "a") },
            { new WindowExample(Text: "a", Required: "aa", Expected: "") },
            { new WindowExample(Text: "ab", Required: "b", Expected: "b") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinWindowByBruteForce_LeetCodeExamples_ReturnsSmallestCoveringSubstring(WindowExample example)
    {
        var actual = MinimumWindowSubstringSolution.MinWindowByBruteForce(
            new MinimumWindowSubstringSolution.SearchedText(example.Text),
            new MinimumWindowSubstringSolution.RequiredCharacters(example.Required));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinWindowBySlidingWindowHashMap_LeetCodeExamples_ReturnsSmallestCoveringSubstring(
        WindowExample example)
    {
        var actual = MinimumWindowSubstringSolution.MinWindowBySlidingWindowHashMap(
            new MinimumWindowSubstringSolution.SearchedText(example.Text),
            new MinimumWindowSubstringSolution.RequiredCharacters(example.Required));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the text to search, the characters that window has to
    // cover, and the smallest covering window. Text and Required are both `string`
    // and the containment is one-directional, so the row names the roles rather than
    // leaving two adjacent positions a caller could swap unnoticed.
    public readonly record struct WindowExample(string Text, string Required, string Expected);
}
