using DSAExperimentation.LeetCode.WordLadderII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordLadderII;

// Harness only: both of WordLadderIISolution's strategies over the same examples.
// LeetCode does not fix an order on the returned sequences, so each result is
// sorted before comparison. The two ends of the ladder are both strings and the
// search is not symmetric in them, so each row names which is which rather than
// leaving two interchangeable positions.
public sealed class WordLadderIITests
{
    public static TheoryData<LadderExample> Examples =>
        new()
        {
            {
                new LadderExample(
                    BeginWord: "hit",
                    EndWord: "cog",
                    WordList: ["hot", "dot", "dog", "lot", "log", "cog"],
                    Expected:
                    [
                        ["hit", "hot", "dot", "dog", "cog"],
                        ["hit", "hot", "lot", "log", "cog"]
                    ])
            },
            {
                new LadderExample(
                    BeginWord: "hit",
                    EndWord: "cog",
                    WordList: ["hot", "dot", "dog", "lot", "log"],
                    Expected: [])
            },
            {
                new LadderExample(
                    BeginWord: "a", EndWord: "c", WordList: ["a", "b", "c"], Expected: [["a", "c"]])
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLaddersByLayeredMutation_LeetCodeExamples_ReturnsEveryShortestSequence(
        LadderExample example)
    {
        var actual = WordLadderIISolution.FindLaddersByLayeredMutation(
            new WordLadderIISolution.BeginWord(example.BeginWord),
            new WordLadderIISolution.EndWord(example.EndWord),
            example.WordList);

        AssertSameSequences(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLaddersByReduceGraph_LeetCodeExamples_ReturnsEveryShortestSequence(
        LadderExample example)
    {
        var actual = WordLadderIISolution.FindLaddersByReduceGraph(
            new WordLadderIISolution.BeginWord(example.BeginWord),
            new WordLadderIISolution.EndWord(example.EndWord),
            example.WordList);

        AssertSameSequences(example.Expected, actual);
    }

    private static void AssertSameSequences(string[][] expected, List<string[]> actual) =>
        Assert.Equal(Sorted(expected), Sorted(actual));

    private static string[][] Sorted(IEnumerable<string[]> sequences) =>
        [.. sequences.OrderBy(sequence => string.Join(',', sequence), StringComparer.Ordinal)];

    // One LeetCode example: the word the ladder starts from, the word it must reach, and
    // the dictionary words it may step through on the way. The two end words are the same
    // type and the search is not symmetric in them, so the row names which is which rather
    // than leaving two interchangeable positions. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct LadderExample(
        string BeginWord, string EndWord, string[] WordList, string[][] Expected);
}
