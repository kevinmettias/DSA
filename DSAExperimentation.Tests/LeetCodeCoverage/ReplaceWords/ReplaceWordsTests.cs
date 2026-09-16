using DSAExperimentation.LeetCode.ReplaceWords;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReplaceWords;

// Harness only: both strategies live in ReplaceWordsSolution and are asserted
// against the same examples.
public sealed partial class ReplaceWordsTests
{
    public static TheoryData<ReplaceWordsExample> Examples =>
        new()
        {
            new ReplaceWordsExample(
                Roots: ["cat", "bat", "rat"],
                Sentence: "the cattle was rattled by the battery",
                Expected: "the cat was rat by the bat"),
            new ReplaceWordsExample(
                Roots: ["a", "b", "c"],
                Sentence: "aadsfasf absbs bbab cadsfafs",
                Expected: "a a b c"),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReplaceByDictionaryScan_LeetCodeExamples_ReplacesEachWordWithItsShortestRoot(
        ReplaceWordsExample example)
    {
        var actual = ReplaceWordsSolution.ReplaceByDictionaryScan(example.Roots, example.Sentence);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReplaceByTrieWalk_LeetCodeExamples_ReplacesEachWordWithItsShortestRoot(
        ReplaceWordsExample example)
    {
        var actual = ReplaceWordsSolution.ReplaceByTrieWalk(example.Roots, example.Sentence);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the roots to replace with, the sentence to replace in,
    // and the sentence that comes back. The sentence and the expected sentence are
    // adjacent string positions at the call site, so the bundle names each one.
    public readonly record struct ReplaceWordsExample(string[] Roots, string Sentence, string Expected);
}
