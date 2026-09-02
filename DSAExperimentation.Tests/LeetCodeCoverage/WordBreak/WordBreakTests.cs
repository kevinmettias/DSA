using DSAExperimentation.LeetCode.WordBreak;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordBreak;

// Harness only. The Trie<bool> + Memoizer composition is WordBreakSolution's -
// this file just pins it to LeetCode's published examples.
public sealed class WordBreakTests
{
    public static TheoryData<string, string[], bool> Examples =>
        new()
        {
            { "leetcode", ["leet", "code"], true },
            { "applepenapple", ["apple", "pen"], true },
            { "catsandog", ["cats", "dog", "sand", "and", "cat"], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBreakByTrieMemoized_LeetCodeExamples_ReturnsWhetherSegmentable(
        string s, string[] wordDict, bool expected) =>
        Assert.Equal(expected, WordBreakSolution.CanBreakByTrieMemoized(s, wordDict));
}
