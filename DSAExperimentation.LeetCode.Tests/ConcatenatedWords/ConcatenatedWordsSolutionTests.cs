using DSAExperimentation.LeetCode.ConcatenatedWords;

namespace DSAExperimentation.LeetCode.Tests.ConcatenatedWords;

// Harness only. Both strategies are ConcatenatedWordsSolution's - this file just
// pins them to LeetCode's published examples and one word list with no answer.
// LeetCode accepts the answer in any order, so both sides compare as sets.
public sealed partial class ConcatenatedWordsSolutionTests
{
    public static TheoryData<string[], string[]> Examples =>
        new()
        {
            // LeetCode examples 1 and 2.
            {
                ["cat", "cats", "catsdogcats", "dog", "dogcatsdog", "hippopotamuses", "rat", "ratcatdogcat"],
                ["catsdogcats", "dogcatsdog", "ratcatdogcat"]
            },
            { ["cat", "dog", "catdog"], ["catdog"] },

            // Three three-letter words: none is long enough to hold two others.
            { ["cat", "dog", "rat"], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllByHashSetScan_LeetCodeExamples_ReturnsWordsBuiltFromShorterOnes(
        string[] words, string[] expected) =>
        Assert.Equal(expected.ToHashSet(), ConcatenatedWordsSolution.FindAllByHashSetScan(words).ToHashSet());

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllByTriePrunedMemo_LeetCodeExamples_ReturnsWordsBuiltFromShorterOnes(
        string[] words, string[] expected) =>
        Assert.Equal(expected.ToHashSet(), ConcatenatedWordsSolution.FindAllByTriePrunedMemo(words).ToHashSet());
}
