using DSAExperimentation.LeetCode.WordLadder;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordLadder;

// Harness only: the one-letter-apart graph is DataStructures.Graph.Hamming's and both search
// strategies are WordLadderSolution's. The two ends of the ladder are both strings and the
// search is not symmetric in them, so each row names which is which rather than leaving two
// interchangeable positions.
public sealed partial class WordLadderTests
{
    public static TheoryData<LadderExample> Examples =>
        new()
        {
            {
                new LadderExample(
                    BeginWord: "hit",
                    EndWord: "cog",
                    WordList: ["hot", "dot", "dog", "lot", "log", "cog"],
                    Expected: 5)
            },
            {
                new LadderExample(
                    BeginWord: "hit",
                    EndWord: "cog",
                    WordList: ["hot", "dot", "dog", "lot", "log"],
                    Expected: 0)
            },
            {
                new LadderExample(
                    BeginWord: "a", EndWord: "c", WordList: ["a", "b", "c"], Expected: 2)
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LadderLengthByMutationQueue_LeetCodeExamples_ReturnsShortestSequenceWordCount(
        LadderExample example)
    {
        var actual = WordLadderSolution.LadderLengthByMutationQueue(
            new BeginWord(example.BeginWord), new EndWord(example.EndWord), example.WordList);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LadderLengthByReduceGraph_LeetCodeExamples_ReturnsShortestSequenceWordCount(
        LadderExample example)
    {
        var actual = WordLadderSolution.LadderLengthByReduceGraph(
            new BeginWord(example.BeginWord), new EndWord(example.EndWord), example.WordList);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the word the ladder starts from, the word it must reach, and
    // the dictionary words it may step through on the way. The two end words are the same
    // type and the search is not symmetric in them, so the row names which is which rather
    // than leaving two interchangeable positions. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct LadderExample(
        string BeginWord, string EndWord, string[] WordList, int Expected);
}
