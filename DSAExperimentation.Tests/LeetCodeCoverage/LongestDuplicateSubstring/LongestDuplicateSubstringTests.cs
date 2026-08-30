using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestDuplicateSubstring;

// LeetCode 1044. Longest Duplicate Substring: the longest duplicated substring is
// exactly the longest shared prefix between two ADJACENT suffixes in sorted
// order - any two non-adjacent suffixes share at most the minimum longest-common-
// prefix value along the sorted run between them, so the true global maximum is
// always attained at some adjacent pair. This repo's own SuffixArray already
// computes every adjacent pair's shared-prefix length via Kasai's algorithm
// (LongestCommonPrefixArray), so the answer is just that array's maximum entry and
// the substring it names - no separate duplicate-detection pass needed.
public sealed partial class LongestDuplicateSubstringTests
{
    [Fact]
    public void LongestDupSubstring_ClassicExample_ReturnsRepeatedSubstring()
    {
        Assert.Equal("ana", LongestDupSubstring("banana"));
    }

    [Fact]
    public void LongestDupSubstring_NoRepeatedSubstring_ReturnsEmptyString()
    {
        Assert.Equal("", LongestDupSubstring("abcd"));
    }

    [Fact]
    public void LongestDupSubstring_OverlappingRepeats_ReturnsLongestOverlap()
    {
        Assert.Equal("aaaa", LongestDupSubstring("aaaaa"));
    }

    private static string LongestDupSubstring(string s)
    {
        var suffixArray = new SuffixArrayStructure(s);
        var longestCommonPrefixes = suffixArray.LongestCommonPrefixArray;

        var bestLength = 0;
        var bestSuffixIndex = -1;

        for (var i = 0; i < longestCommonPrefixes.Length; i++)
        {
            if (longestCommonPrefixes[i] > bestLength)
            {
                bestLength = longestCommonPrefixes[i];
                bestSuffixIndex = i;
            }
        }

        return bestLength == 0 ? "" : s.Substring(suffixArray.Suffixes[bestSuffixIndex], bestLength);
    }
}
