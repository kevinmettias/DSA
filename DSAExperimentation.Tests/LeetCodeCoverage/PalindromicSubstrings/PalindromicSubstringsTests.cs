using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromicSubstrings;

// LeetCode 647. Palindromic Substrings: this repo's Manacher radii already count
// every palindromic substring, not just the longest one - Manacher.ComputeOddRadii
// (text)[i] = k means k nested odd-length palindromes are centered at i (lengths
// 1, 3, .., 2k-1), and ComputeEvenRadii(text)[i] = k means k nested even-length
// palindromes are centered between i-1 and i, so summing every radius counts every
// palindromic substring in the text in one O(n) pass. Same primitive
// LongestPalindromicSubstringTests already reuses, read differently: that test
// takes the max radius, this one takes the sum.
public sealed partial class PalindromicSubstringsTests
{
    [Fact]
    public void CountSubstrings_AllDistinctCharacters_CountsOnlySingleCharacterPalindromes()
        => Assert.Equal(3, CountSubstrings("abc")); // "a", "b", "c"

    [Fact]
    public void CountSubstrings_RepeatedCharacter_CountsNestedAndOverlappingPalindromes()
        => Assert.Equal(6, CountSubstrings("aaa")); // "a","a","a","aa","aa","aaa"

    [Fact]
    public void CountSubstrings_MixedPalindrome_CountsSingleAndMultiCharacterPalindromes()
        => Assert.Equal(4, CountSubstrings("aba")); // "a","b","a","aba"

    [Fact]
    public void CountSubstrings_SingleCharacter_ReturnsOne()
        => Assert.Equal(1, CountSubstrings("a"));

    private static int CountSubstrings(string s)
    {
        var count = 0;

        foreach (var radius in Manacher.ComputeOddRadii(s))
        {
            count += radius;
        }

        foreach (var radius in Manacher.ComputeEvenRadii(s))
        {
            count += radius;
        }

        return count;
    }
}
