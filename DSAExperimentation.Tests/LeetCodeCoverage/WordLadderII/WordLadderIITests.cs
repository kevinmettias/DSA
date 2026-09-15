using DSAExperimentation.LeetCode.WordLadderII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordLadderII;

// Harness only: both of WordLadderIISolution's strategies over the same examples.
// LeetCode does not fix an order on the returned sequences, so each result is
// sorted before comparison.
public sealed class WordLadderIITests
{
    public static TheoryData<string, string, string[], string[][]> Examples =>
        new()
        {
            {
                "hit", "cog", ["hot", "dot", "dog", "lot", "log", "cog"],
                [["hit", "hot", "dot", "dog", "cog"], ["hit", "hot", "lot", "log", "cog"]]
            },
            { "hit", "cog", ["hot", "dot", "dog", "lot", "log"], [] },
            { "a", "c", ["a", "b", "c"], [["a", "c"]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLaddersByLayeredMutation_LeetCodeExamples_ReturnsEveryShortestSequence(
        string beginWord, string endWord, string[] wordList, string[][] expected) =>
        AssertSameSequences(
            expected,
            WordLadderIISolution.FindLaddersByLayeredMutation(
                new WordLadderIISolution.BeginWord(beginWord),
                new WordLadderIISolution.EndWord(endWord),
                wordList));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLaddersByReduceGraph_LeetCodeExamples_ReturnsEveryShortestSequence(
        string beginWord, string endWord, string[] wordList, string[][] expected) =>
        AssertSameSequences(
            expected,
            WordLadderIISolution.FindLaddersByReduceGraph(
                new WordLadderIISolution.BeginWord(beginWord),
                new WordLadderIISolution.EndWord(endWord),
                wordList));

    private static void AssertSameSequences(string[][] expected, List<string[]> actual) =>
        Assert.Equal(Sorted(expected), Sorted(actual));

    private static string[][] Sorted(IEnumerable<string[]> sequences) =>
        [.. sequences.OrderBy(sequence => string.Join(',', sequence), StringComparer.Ordinal)];
}
