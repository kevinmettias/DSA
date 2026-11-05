using DSAExperimentation.LeetCode.Solutions.LongestPalindromicSubstring;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestPalindromicSubstring;

// Harness only. Both strategies are LongestPalindromicSubstringSolution's - this
// file pins them to a string with no palindrome longer than one character, where
// the first character is the answer. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class LongestPalindromicSubstringSolutionTests
{
    public static TheoryData<PalindromeCase> Cases =>
        new()
        {
            { new PalindromeCase(S: "abcd", Expected: "a") },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindLongestPalindromeByExpandAroundCenter_Cases_ReturnsLongestPalindrome(
        PalindromeCase testCase) =>
        Assert.Equal(testCase.Expected, LongestPalindromicSubstringSolution.FindLongestPalindromeByExpandAroundCenter(testCase.S));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindLongestPalindromeByManacher_Cases_ReturnsLongestPalindrome(
        PalindromeCase testCase) =>
        Assert.Equal(testCase.Expected, LongestPalindromicSubstringSolution.FindLongestPalindromeByManacher(testCase.S));

    // One case as one argument. The input and the answer are both strings, so a
    // two-parameter signature let a row be written with the two swapped and still
    // compile; the fields named at each row below say which is which.
    public readonly record struct PalindromeCase(string S, string Expected);
}
