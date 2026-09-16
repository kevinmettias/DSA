using DSAExperimentation.LeetCode.WordBreakII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordBreakII;

// Harness only. Both strategies are WordBreakIISolution's - this file just pins
// them to LeetCode's published examples, sorting each result the same way the
// original assertions did since neither strategy promises a particular sentence
// order.
public sealed class WordBreakIITests
{
    public static TheoryData<string, string[], string[]> Examples =>
        new()
        {
            {
                "catsanddog", ["cat", "cats", "and", "sand", "dog"],
                ["cat sand dog", "cats and dog"]
            },
            {
                "pineapplepenapple", ["apple", "pen", "applepen", "pine", "pineapple"],
                ["pine apple pen apple", "pine applepen apple", "pineapple pen apple"]
            },
            {
                "catsandog", ["cats", "dog", "sand", "and", "cat"],
                []
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SentencesByHashSetScan_LeetCodeExamples_ReturnsEverySegmentation(
        string source, string[] wordDict, string[] expected) =>
        Assert.Equal(
            expected.OrderBy(x => x),
            WordBreakIISolution.SentencesByHashSetScan(source, wordDict).OrderBy(x => x));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SentencesByTrieMemoized_LeetCodeExamples_ReturnsEverySegmentation(
        string source, string[] wordDict, string[] expected) =>
        Assert.Equal(
            expected.OrderBy(x => x),
            WordBreakIISolution.SentencesByTrieMemoized(source, wordDict).OrderBy(x => x));
}
