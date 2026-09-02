using DSAExperimentation.LeetCode.LongestPalindromicSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestPalindromicSubstring;

// Harness only. Both strategies are LongestPalindromicSubstringSolution's - this
// file just pins them to LeetCode's published examples.
public sealed class LongestPalindromicSubstringTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "babad", "bab" }, // "aba" ties in length
            { "cbbd", "bb" },
            { "racecar", "racecar" },
            { "a", "a" },
            { "abcd", "a" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestPalindromeByExpandAroundCenter_LeetCodeExamples_ReturnsLongestPalindrome(
        string s, string expected) =>
        Assert.Equal(expected, LongestPalindromicSubstringSolution.FindLongestPalindromeByExpandAroundCenter(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestPalindromeByManacher_LeetCodeExamples_ReturnsLongestPalindrome(
        string s, string expected) =>
        Assert.Equal(expected, LongestPalindromicSubstringSolution.FindLongestPalindromeByManacher(s));
}
