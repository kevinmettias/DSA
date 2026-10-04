using DSAExperimentation.LeetCode.WordSearchII;

namespace DSAExperimentation.LeetCode.Tests.WordSearchII;

// Harness only. Both search strategies are WordSearchIISolution's - this file just
// pins them to LeetCode's published examples, plus two boards of its own: one where
// no word is present, and the prefix-sharing case where one dictionary word is
// itself a prefix of another. LeetCode accepts the words in any order.
public sealed partial class WordSearchIISolutionTests
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
            // LeetCode examples 1 and 2.
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
            { [['a', 'b'], ['c', 'd']], ["abcb"], [] },

            // The board holds no 'o' and no 't', so neither word can be traced; "a" is
            // the top-left cell and "ab" steps right from it, so both are found.
            { [['a', 'b'], ['c', 'd']], ["dog", "cat"], [] },
            { [['a', 'b'], ['c', 'd']], ["a", "ab"], ["a", "ab"] },
        };
}
