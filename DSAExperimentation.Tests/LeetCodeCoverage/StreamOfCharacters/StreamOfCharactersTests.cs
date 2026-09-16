using DSAExperimentation.LeetCode.StreamOfCharacters;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StreamOfCharacters;

// Harness only. Both strategies are StreamOfCharactersSolution's - this file streams
// each example's characters one at a time through an IStreamCheckerStrategy and
// checks the per-character answers LeetCode itself publishes, so a failure still
// names the strategy that broke.
public sealed class StreamOfCharactersTests
{
    public static TheoryData<string[], string, bool[]> Examples =>
        new()
        {
            {
                ["cd", "f", "kl"],
                "abcdefghijkl",
                new[] { false, false, false, true, false, true, false, false, false, false, false, true }
            },
            {
                ["ab", "ba"],
                "abab",
                new[] { false, true, true, true }
            },
            {
                ["a"],
                "ba",
                new[] { false, true }
            },
            {
                ["abc"],
                "abcd",
                new[] { false, false, true, false }
            },
            {
                Array.Empty<string>(),
                "ab",
                new[] { false, false }
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void StreamCheckerBySuffixRescan_LeetCodeExamples_ReportsWhetherAnySuffixSpellsAWord(
        string[] words, string stream, bool[] expected) =>
        AssertQueryResults(
            new StreamOfCharactersSolution.StreamCheckerBySuffixRescan(words),
            stream,
            expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void StreamCheckerByReversedTrie_LeetCodeExamples_ReportsWhetherAnySuffixSpellsAWord(
        string[] words, string stream, bool[] expected) =>
        AssertQueryResults(
            new StreamOfCharactersSolution.StreamCheckerByReversedTrie(words),
            stream,
            expected);

    private static void AssertQueryResults(
        StreamOfCharactersSolution.IStreamCheckerStrategy checker,
        string stream,
        bool[] expected)
    {
        for (var i = 0; i < stream.Length; i++)
        {
            Assert.Equal(expected[i], checker.Query(stream[i]));
        }
    }
}
