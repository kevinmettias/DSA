using DSAExperimentation.LeetCode.SmallestPalindromicRearrangementII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestPalindromicRearrangementII;

// Harness only. Both strategies are SmallestPalindromicRearrangementIISolution's -
// this file just pins them to LeetCode's published examples.
public sealed class SmallestPalindromicRearrangementIITests
{
    public static TheoryData<string, int, string> Examples =>
        new()
        {
            { "abba", 2, "baab" },
            { "aa", 2, "" },
            { "bacab", 1, "abcba" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RearrangeByBacktrackingRank_LeetCodeExamples_ReturnsKthSmallestPalindrome(
        string s, int k, string expected) =>
        Assert.Equal(expected, SmallestPalindromicRearrangementIISolution.RearrangeByBacktrackingRank(s, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RearrangeByCountingGreedy_LeetCodeExamples_ReturnsKthSmallestPalindrome(
        string s, int k, string expected) =>
        Assert.Equal(expected, SmallestPalindromicRearrangementIISolution.RearrangeByCountingGreedy(s, k));
}
