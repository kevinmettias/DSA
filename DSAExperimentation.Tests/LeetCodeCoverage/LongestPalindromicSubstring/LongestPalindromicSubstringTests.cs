using DSAExperimentation.LeetCode.LongestPalindromicSubstring;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestPalindromicSubstring;

// Harness only. Both strategies are LongestPalindromicSubstringSolution's - this
// file just pins them to LeetCode's published examples.
public sealed partial class LongestPalindromicSubstringTests
{
    public static TheoryData<PalindromeExample> Examples =>
        new()
        {
            { new PalindromeExample(S: "babad", Expected: "bab") }, // "aba" ties in length
            { new PalindromeExample(S: "cbbd", Expected: "bb") },
            { new PalindromeExample(S: "racecar", Expected: "racecar") },
            { new PalindromeExample(S: "a", Expected: "a") },
            { new PalindromeExample(S: "abcd", Expected: "a") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestPalindromeByExpandAroundCenter_LeetCodeExamples_ReturnsLongestPalindrome(
        PalindromeExample example) =>
        Assert.Equal(example.Expected, LongestPalindromicSubstringSolution.FindLongestPalindromeByExpandAroundCenter(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindLongestPalindromeByManacher_LeetCodeExamples_ReturnsLongestPalindrome(
        PalindromeExample example) =>
        Assert.Equal(example.Expected, LongestPalindromicSubstringSolution.FindLongestPalindromeByManacher(example.S));

    // One example as one argument. The input and the answer are both strings, so a
    // two-parameter signature let a row be written with the two swapped and still
    // compile; the fields named at each row below say which is which.
    public readonly record struct PalindromeExample(string S, string Expected);
}
