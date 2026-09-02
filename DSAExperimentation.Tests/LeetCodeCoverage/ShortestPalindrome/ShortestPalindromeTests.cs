using DSAExperimentation.LeetCode.ShortestPalindrome;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPalindrome;

// Harness only. Both strategies are ShortestPalindromeSolution's - this file just
// pins them to LeetCode's published examples plus the edge cases the original
// coverage carried (empty string, single char, already-palindrome).
public sealed class ShortestPalindromeTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "aacecaaa", "aaacecaaa" },
            { "abcd", "dcbabcd" },
            { "", "" },
            { "a", "a" },
            { "aa", "aa" },
            { "racecar", "racecar" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildShortestPalindromeByNaiveScan_LeetCodeExamples_PrependsFewestCharacters(
        string s, string expected) =>
        Assert.Equal(expected, ShortestPalindromeSolution.BuildShortestPalindromeByNaiveScan(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildShortestPalindromeByKmpFailureFunction_LeetCodeExamples_PrependsFewestCharacters(
        string s, string expected) =>
        Assert.Equal(expected, ShortestPalindromeSolution.BuildShortestPalindromeByKmpFailureFunction(s));
}
