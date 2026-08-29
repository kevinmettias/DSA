using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestPalindromicSubstring;

// LeetCode 5. Longest Palindromic Substring: this repo's own O(n) Manacher primitive
// already returns exactly the (Start, Length) bounds this problem asks for - solving
// it is just slicing the input at those bounds, not a new algorithm.
public sealed partial class LongestPalindromicSubstringTests
{
    [Fact]
    public void FindLongestPalindrome_MultiplePalindromesPresent_ReturnsLongestOne()
        => Assert.Equal("bab", FindLongestPalindrome("babad")); // "aba" ties in length

    [Fact]
    public void FindLongestPalindrome_EvenLengthPalindrome_ReturnsIt()
        => Assert.Equal("bb", FindLongestPalindrome("cbbd"));

    [Fact]
    public void FindLongestPalindrome_WholeStringIsAPalindrome_ReturnsWholeString()
        => Assert.Equal("racecar", FindLongestPalindrome("racecar"));

    [Fact]
    public void FindLongestPalindrome_SingleCharacter_ReturnsThatCharacter()
        => Assert.Equal("a", FindLongestPalindrome("a"));

    [Fact]
    public void FindLongestPalindrome_NoRepeatedCharacters_ReturnsFirstCharacterOnly()
        => Assert.Equal("a", FindLongestPalindrome("abcd"));

    private static string FindLongestPalindrome(string s)
    {
        var (start, length) = Manacher.FindLongestPalindromicSubstring(s);
        return s.Substring(start, length);
    }
}
