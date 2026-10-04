using DSAExperimentation.DataStructures.Trie;
using DSAExperimentation.LeetCode.LongestCommonSuffixQueries;

namespace DSAExperimentation.LeetCode.Tests.LongestCommonSuffixQueries;

// Harness only. The suffix trie itself is LongestCommonSuffixQueriesSolution's -
// this file pins both strategies to LeetCode's published examples, including the
// "xyz"/all-tie-at-empty-suffix case that exercises the trie's root value, and the
// trie to the best word it should record under every reversed suffix.
public sealed partial class LongestCommonSuffixQueriesSolutionTests
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
        string[] wordsContainer, string[] wordsQuery, int[] expected)
    {
        var actual = LongestCommonSuffixQueriesSolution.FindIndicesByBruteForce(wordsContainer, wordsQuery);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindIndicesByTrie_LeetCodeExamples_ReturnsLongestCommonSuffixIndices(
        string[] wordsContainer, string[] wordsQuery, int[] expected)
    {
        var actual = LongestCommonSuffixQueriesSolution.FindIndicesByTrie(wordsContainer, wordsQuery);

        Assert.Equal(expected, actual);
    }

    // LeetCode's first container, words reversed: "dcba" (0), "dcb" (1), "dcbx" (2). The
    // empty key holds the shortest word, 1. "abcd" claims d, dc, dcb and dcba first; the
    // strictly shorter "bcd" takes d, dc and dcb over; "xbcd" is longer than "bcd", so it
    // adds only dcbx. Six keys in all.
    [Fact]
    public void BuildSuffixTrie_LeetCodeFirstExample_RecordsTheShortestWordUnderEachReversedSuffix()
    {
        var trie = LongestCommonSuffixQueriesSolution.BuildSuffixTrie(["abcd", "bcd", "xbcd"]);
        var bestWords = new[] { "", "d", "dc", "dcb", "dcba", "dcbx" }.Select(key => BestWordAt(trie, key));

        Assert.Equal(6, trie.Count);
        Assert.Equal([1, 1, 1, 1, 0, 2], bestWords);
    }

    // Two words of equal length share the suffix "b": only a strictly shorter word
    // overwrites a key, so the tie at "" and at "b" keeps the earlier index, 0, and
    // each word alone owns its full reversal, "ba" and "bc".
    [Fact]
    public void BuildSuffixTrie_EqualLengthTie_KeepsTheEarlierIndex()
    {
        var trie = LongestCommonSuffixQueriesSolution.BuildSuffixTrie(["ab", "cb"]);
        var bestWords = new[] { "", "b", "ba", "bc" }.Select(key => BestWordAt(trie, key));

        Assert.Equal(4, trie.Count);
        Assert.Equal([0, 0, 0, 1], bestWords);
    }

    private static int BestWordAt(Trie<int> trie, string reversedSuffix)
    {
        var found = trie.TryGetValue(reversedSuffix, out var wordIndex);

        Assert.True(found);
        return wordIndex;
    }
}
