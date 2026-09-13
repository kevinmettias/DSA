using DSAExperimentation.LeetCode.MaximumRepeatingSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumRepeatingSubstring;

// Harness only. Both substring-search strategies live in
// MaximumRepeatingSubstringSolution and are asserted against the same examples,
// including the case where word never occurs at all (answer 0) and one where the
// whole sequence is a single repetition of word.
public sealed class MaximumRepeatingSubstringTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "ababc", "ab", 2 },
            { "ababc", "ba", 1 },
            { "ababc", "ac", 0 },
            { "aaabaaaabaaabaaaabaaaabaaaabaaaaba", "aaaba", 5 },
            { "a", "ab", 0 },
            { "abababab", "ab", 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxRepeatingByStringContains_LeetCodeExamples_ReturnsMaximumRepeatCount(
        string sequence, string word, int expected) =>
        Assert.Equal(expected, MaximumRepeatingSubstringSolution.MaxRepeatingByStringContains(sequence, word));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxRepeatingByPrefixFunctionSearch_LeetCodeExamples_ReturnsMaximumRepeatCount(
        string sequence, string word, int expected) =>
        Assert.Equal(expected, MaximumRepeatingSubstringSolution.MaxRepeatingByPrefixFunctionSearch(sequence, word));
}
