using DSAExperimentation.LeetCode.WordSearchII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordSearchII;

// Harness only. Both search strategies are WordSearchIISolution's - this file just
// pins them to LeetCode's published examples, including the prefix-sharing case
// where one dictionary word is itself a prefix of another.
public sealed partial class WordSearchIITests
{
    [Theory]
    [MemberData(nameof(Examples))]
    public void FindWordsByBruteForceDfs_LeetCodeExamples_ReturnsEveryWordPresentOnTheBoard(
        char[][] board, string[] words, string[] expected) =>
        Assert.Equal(expected.Order(), WordSearchIISolution.FindWordsByBruteForceDfs(board, words).Order());

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindWordsByTrieBacktrack_LeetCodeExamples_ReturnsEveryWordPresentOnTheBoard(
        char[][] board, string[] words, string[] expected) =>
        Assert.Equal(expected.Order(), WordSearchIISolution.FindWordsByTrieBacktrack(board, words).Order());

    public static TheoryData<char[][], string[], string[]> Examples =>
        new()
        {
            {
                [
                    ['o', 'a', 'a', 'n'],
                    ['e', 't', 'a', 'e'],
                    ['i', 'h', 'k', 'r'],
                    ['i', 'f', 'l', 'v'],
                ],
                ["oath", "pea", "eat", "rain"],
                ["eat", "oath"]
            },
            { [['a', 'b'], ['c', 'd']], ["dog", "cat"], [] },
            { [['a', 'b'], ['c', 'd']], ["a", "ab"], ["a", "ab"] },
        };
}
