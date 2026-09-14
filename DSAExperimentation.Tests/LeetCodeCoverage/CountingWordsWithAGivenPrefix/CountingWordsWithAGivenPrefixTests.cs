using DSAExperimentation.LeetCode.CountingWordsWithAGivenPrefix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountingWordsWithAGivenPrefix;

// Harness only. Both the StartsWith scan and the Trie<bool> insert-then-HasPrefix round
// trip are CountingWordsWithAGivenPrefixSolution's; this file pins them to LeetCode's
// published examples plus the boundary cases the two have to agree on - a prefix equal
// to a whole word, a prefix longer than any word, repeated words that must each be
// counted, and a prefix that occurs inside words without ever starting one.
public sealed class CountingWordsWithAGivenPrefixTests
{
    public static TheoryData<string[], string, int> Examples =>
        new()
        {
            { ["pay", "attention", "practice", "attend"], "at", 2 },
            { ["leetcode", "win", "loops", "success"], "code", 0 },
            { ["i", "love", "leetcode"], "i", 1 },
            { ["a", "b", "c"], "abc", 0 },
            { ["pay", "attention", "practice", "attend"], "pra", 1 },
            { ["at", "at", "att"], "at", 3 },
            { ["hellohello", "hellohellohello"], "ell", 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWordsWithPrefixByStartsWithScan_LeetCodeExamples_ReturnsMatchingWordCount(
        string[] words, string pref, int expected) =>
        Assert.Equal(
            expected,
            CountingWordsWithAGivenPrefixSolution.CountWordsWithPrefixByStartsWithScan(words, pref));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWordsWithPrefixByTriePerWord_LeetCodeExamples_ReturnsMatchingWordCount(
        string[] words, string pref, int expected) =>
        Assert.Equal(
            expected,
            CountingWordsWithAGivenPrefixSolution.CountWordsWithPrefixByTriePerWord(words, pref));
}
