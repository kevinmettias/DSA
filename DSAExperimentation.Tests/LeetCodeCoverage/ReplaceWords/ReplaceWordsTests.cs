using DSAExperimentation.LeetCode.ReplaceWords;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReplaceWords;

// Harness only: both strategies live in ReplaceWordsSolution and are asserted
// against the same examples.
public sealed class ReplaceWordsTests
{
    public static TheoryData<string[], string, string> Examples =>
        new()
        {
            { ["cat", "bat", "rat"], "the cattle was rattled by the battery", "the cat was rat by the bat" },
            { ["a", "b", "c"], "aadsfasf absbs bbab cadsfafs", "a a b c" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReplaceByDictionaryScan_LeetCodeExamples_ReplacesEachWordWithItsShortestRoot(
        string[] dictionary, string sentence, string expected) =>
        Assert.Equal(expected, ReplaceWordsSolution.ReplaceByDictionaryScan(dictionary, sentence));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReplaceByTrieWalk_LeetCodeExamples_ReplacesEachWordWithItsShortestRoot(
        string[] dictionary, string sentence, string expected) =>
        Assert.Equal(expected, ReplaceWordsSolution.ReplaceByTrieWalk(dictionary, sentence));
}
