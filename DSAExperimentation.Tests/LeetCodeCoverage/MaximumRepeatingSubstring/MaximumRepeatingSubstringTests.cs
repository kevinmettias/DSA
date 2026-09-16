using DSAExperimentation.LeetCode.MaximumRepeatingSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumRepeatingSubstring;

// Harness only. Both substring-search strategies live in
// MaximumRepeatingSubstringSolution and are asserted against the same examples,
// including the case where word never occurs at all (answer 0) and one where the
// whole sequence is a single repetition of word.
public sealed partial class MaximumRepeatingSubstringTests
{
    public static TheoryData<RepeatCountExample> Examples =>
        new()
        {
            { new RepeatCountExample(Sequence: "ababc", Word: "ab", Expected: 2) },
            { new RepeatCountExample(Sequence: "ababc", Word: "ba", Expected: 1) },
            { new RepeatCountExample(Sequence: "ababc", Word: "ac", Expected: 0) },
            { new RepeatCountExample(Sequence: "aaabaaaabaaabaaaabaaaabaaaabaaaaba", Word: "aaaba", Expected: 5) },
            { new RepeatCountExample(Sequence: "a", Word: "ab", Expected: 0) },
            { new RepeatCountExample(Sequence: "abababab", Word: "ab", Expected: 4) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxRepeatingByStringContains_LeetCodeExamples_ReturnsMaximumRepeatCount(
        RepeatCountExample example)
    {
        var actual = MaximumRepeatingSubstringSolution.MaxRepeatingByStringContains(
            new MaximumRepeatingSubstringSolution.Haystack(example.Sequence),
            new MaximumRepeatingSubstringSolution.RepeatedWord(example.Word));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxRepeatingByPrefixFunctionSearch_LeetCodeExamples_ReturnsMaximumRepeatCount(
        RepeatCountExample example)
    {
        var actual = MaximumRepeatingSubstringSolution.MaxRepeatingByPrefixFunctionSearch(
            new MaximumRepeatingSubstringSolution.Haystack(example.Sequence),
            new MaximumRepeatingSubstringSolution.RepeatedWord(example.Word));

        Assert.Equal(example.Expected, actual);
    }

    // One example as one argument. A sequence and the word counted inside it are both
    // strings, so a multi-parameter signature let a row be written with the two swapped
    // and still compile; the fields named at each row below say which is which.
    public readonly record struct RepeatCountExample(string Sequence, string Word, int Expected);
}
