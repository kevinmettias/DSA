using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;

namespace DSAExperimentation.LeetCode.LongestDuplicateSubstring;

// LeetCode 1044. Longest Duplicate Substring: return the longest substring that
// occurs at least twice in the text (occurrences may overlap), or "" if there is none.
//
// The key property both strategies rest on is that a duplicated substring is
// exactly a shared prefix of two distinct suffixes. The baseline takes that
// literally and compares every pair; the composed strategy uses the fact that the
// longest shared prefix over ALL pairs is always attained by an ADJACENT pair in
// sorted-suffix order - any two non-adjacent suffixes share at most the minimum
// longest-common-prefix value along the sorted run between them - so this repo's
// own SuffixArray, whose Kasai's-algorithm LongestCommonPrefixArray already reports
// every adjacent pair's shared-prefix length, answers the whole problem with a
// single scan of that array and no separate duplicate-detection pass.
internal static class LongestDuplicateSubstringSolution
{
    // Baseline: compare every pair of suffixes directly, O(n^2) pairs each costing
    // an O(match length) character scan. Deliberately plain BCL - this is what you
    // would write without this repo.
    public static string LongestDuplicateSubstringByAllSuffixPairs(string text)
    {
        var bestLength = 0;
        var bestStart = 0;

        for (var i = 0; i < text.Length; i++)
        {
            for (var j = i + 1; j < text.Length; j++)
            {
                var commonPrefixLength = CommonPrefixLength(text, i, j);

                if (commonPrefixLength > bestLength)
                {
                    bestLength = commonPrefixLength;
                    bestStart = i;
                }
            }
        }

        return text.Substring(bestStart, bestLength);
    }

    private static int CommonPrefixLength(string text, int first, int second)
    {
        var length = 0;

        while (HasMatchingCharacterAt(text, first, second, length))
        {
            length++;
        }

        return length;
    }

    // Both suffixes still have a character at this offset, and those characters
    // are the same one.
    private static bool HasMatchingCharacterAt(string text, int first, int second, int offset)
        => first + offset < text.Length
            && second + offset < text.Length
            && text[first + offset] == text[second + offset];

    // Composed: the answer is the maximum entry of the suffix array's
    // longest-common-prefix array, and the substring it names starts at the suffix
    // that entry belongs to. O(n log^2 n) to build, then one linear scan.
    public static string LongestDuplicateSubstringBySuffixArray(string text)
    {
        var suffixArray = new SuffixArrayStructure(text);
        var longestCommonPrefixes = suffixArray.LongestCommonPrefixArray;

        var bestLength = 0;
        var bestSuffixIndex = 0;

        for (var i = 0; i < longestCommonPrefixes.Length; i++)
        {
            if (longestCommonPrefixes[i] > bestLength)
            {
                bestLength = longestCommonPrefixes[i];
                bestSuffixIndex = i;
            }
        }

        return bestLength == 0 ? "" : text.Substring(suffixArray.Suffixes[bestSuffixIndex], bestLength);
    }
}
