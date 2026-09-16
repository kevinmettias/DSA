using DSAExperimentation.LeetCode.ConcatenatedWords;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConcatenatedWords;

// Harness only. Both strategies are ConcatenatedWordsSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class ConcatenatedWordsTests
{
    public static TheoryData<string[], string[]> Examples =>
        new()
        {
            {
                ["cat", "cats", "catsdogcats", "dog", "dogcatsdog", "hippopotamuses", "rat", "ratcatdogcat"],
                ["catsdogcats", "dogcatsdog", "ratcatdogcat"]
            },
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
