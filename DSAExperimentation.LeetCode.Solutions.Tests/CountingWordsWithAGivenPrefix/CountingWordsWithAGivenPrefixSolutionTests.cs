using DSAExperimentation.LeetCode.Solutions.CountingWordsWithAGivenPrefix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountingWordsWithAGivenPrefix;

// Harness only. Both the StartsWith scan and the trie's insert-then-HasPrefix round
// trip are CountingWordsWithAGivenPrefixSolution's; this file pins them to the
// boundary cases the two have to agree on - a prefix equal to a whole word, a prefix
// longer than any word, repeated words that must each be counted, and a prefix that
// occurs inside words without ever starting one - plus the words of LeetCode's first
// example under a prefix only one of them starts with. LeetCode's published examples
// run in CatalogCaseTests.
public sealed partial class CountingWordsWithAGivenPrefixSolutionTests
{
    public static TheoryData<string[], string, int> Cases =>
        new()
        {
            { ["i", "love", "leetcode"], "i", 1 },
            { ["a", "b", "c"], "abc", 0 },
            { ["pay", "attention", "practice", "attend"], "pra", 1 },
            { ["at", "at", "att"], "at", 3 },
            { ["hellohello", "hellohellohello"], "ell", 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWordsWithPrefixByStartsWithScan_Cases_ReturnsMatchingWordCount(
        string[] words, string pref, int expected)
    {
        var actual = CountingWordsWithAGivenPrefixSolution.CountWordsWithPrefixByStartsWithScan(words, pref);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountWordsWithPrefixByTriePerWord_Cases_ReturnsMatchingWordCount(
        string[] words, string pref, int expected)
    {
        var actual = CountingWordsWithAGivenPrefixSolution.CountWordsWithPrefixByTriePerWord(words, pref);
        Assert.Equal(expected, actual);
    }
}
