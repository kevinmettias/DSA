using DSAExperimentation.DataStructures.SuffixArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LastSubstringInLexicographicalOrder;

// LeetCode 1163. Last Substring in Lexicographical Order: the answer is exactly the
// lexicographically greatest suffix of s, which is the LAST entry of this repo's own
// SuffixArray.Suffixes permutation (Suffixes is sorted ascending, so Suffixes[^1] is
// the start index of the maximal suffix) - no separate algorithm needed beyond
// reading that one index back out.
public sealed partial class LastSubstringInLexicographicalOrderTests
{
    [Fact]
    public void LastSubstring_RepeatedCharactersTieBreakOnLength_ReturnsLongestMaximalSuffix()
        => Assert.Equal("bab", FindLastSubstring("abab"));

    [Fact]
    public void LastSubstring_DistinctCharacters_ReturnsSuffixStartingAtMaxCharacter()
        => Assert.Equal("tcode", FindLastSubstring("leetcode"));

    [Fact]
    public void LastSubstring_SingleCharacter_ReturnsTheWholeString()
        => Assert.Equal("z", FindLastSubstring("z"));

    private static string FindLastSubstring(string s)
    {
        var suffixArray = new SuffixArray(s);
        var maxSuffixStart = suffixArray.Suffixes[^1];
        return s[maxSuffixStart..];
    }
}
