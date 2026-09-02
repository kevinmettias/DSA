using DSAExperimentation.LeetCode.WordLadder;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordLadder;

// Harness only: the one-letter-apart graph is DataStructures.Graph.Hamming's and both search
// strategies are WordLadderSolution's.
public sealed class WordLadderTests
{
    public static TheoryData<string, string, string[], int> Examples =>
        new()
        {
            { "hit", "cog", ["hot", "dot", "dog", "lot", "log", "cog"], 5 },
            { "hit", "cog", ["hot", "dot", "dog", "lot", "log"], 0 },
            { "a", "c", ["a", "b", "c"], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LadderLengthByMutationQueue_LeetCodeExamples_ReturnsShortestSequenceWordCount(
        string beginWord, string endWord, string[] wordList, int expected) =>
        Assert.Equal(expected, WordLadderSolution.LadderLengthByMutationQueue(beginWord, endWord, wordList));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LadderLengthByReduceGraph_LeetCodeExamples_ReturnsShortestSequenceWordCount(
        string beginWord, string endWord, string[] wordList, int expected) =>
        Assert.Equal(expected, WordLadderSolution.LadderLengthByReduceGraph(beginWord, endWord, wordList));
}
