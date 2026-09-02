using DSAExperimentation.LeetCode.LongestCommonSuffixQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCommonSuffixQueries;

// Harness only. The suffix trie itself is LongestCommonSuffixQueriesSolution's -
// this file just pins both strategies to LeetCode's published examples, including
// the "xyz"/all-tie-at-empty-suffix case that exercises the trie's root value.
public sealed class LongestCommonSuffixQueriesTests
{
    public static TheoryData<string[], string[], int[]> Examples =>
        new()
        {
            {
                ["abcd", "bcd", "xbcd"],
                ["cd", "bcd", "xyz"],
                [1, 1, 1]
            },
            {
                ["abcdefgh", "poiuygh", "ghghgh"],
                ["gh", "acbfgh", "acbfegh"],
                [2, 0, 2]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindIndicesByBruteForce_LeetCodeExamples_ReturnsLongestCommonSuffixIndices(
        string[] wordsContainer, string[] wordsQuery, int[] expected) =>
        Assert.Equal(expected, LongestCommonSuffixQueriesSolution.FindIndicesByBruteForce(wordsContainer, wordsQuery));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindIndicesByTrie_LeetCodeExamples_ReturnsLongestCommonSuffixIndices(
        string[] wordsContainer, string[] wordsQuery, int[] expected) =>
        Assert.Equal(expected, LongestCommonSuffixQueriesSolution.FindIndicesByTrie(wordsContainer, wordsQuery));
}
