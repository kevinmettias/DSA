using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.LongestPalindromicSubstring;

// LeetCode 5. Longest Palindromic Substring: the longest contiguous run of the input
// that reads the same forwards and backwards.
//
// The baseline is the textbook O(n^2) expand-around-every-center walk, written
// without this repo's primitives. The composed strategy is this repo's own O(n)
// Manacher primitive, which already returns exactly the (Start, Length) bounds this
// problem asks for - solving it is just slicing the input at those bounds, not a new
// algorithm. Both return LeetCode's actual answer (the substring itself); the
// original benchmark arms measured only the winning length, which is promoted here.
internal static class LongestPalindromicSubstringSolution
{
    public static string FindLongestPalindromeByExpandAroundCenter(string s)
    {
        var bestStart = 0;
        var bestLength = 0;

        for (var center = 0; center < s.Length; center++)
        {
            var (oddStart, oddLength) = ExpandFrom(s, center, center);

            if (oddLength > bestLength)
            {
                (bestStart, bestLength) = (oddStart, oddLength);
            }

            var (evenStart, evenLength) = ExpandFrom(s, center, center + 1);

            if (evenLength > bestLength)
            {
                (bestStart, bestLength) = (evenStart, evenLength);
            }
        }

        return s.Substring(bestStart, bestLength);
    }

    private static (int Start, int Length) ExpandFrom(string s, int left, int right)
    {
        while (ExpandsToMatchingPair(s, left, right))
        {
            left--;
            right++;
        }

        return (left + 1, right - left - 1);
    }

    // The walk can keep widening while both ends are still inside the string and
    // the characters they hold are the same.
    private static bool ExpandsToMatchingPair(string s, int left, int right)
        => left >= 0 && right < s.Length && s[left] == s[right];

    public static string FindLongestPalindromeByManacher(string s)
    {
        var (start, length) = Manacher.FindLongestPalindromicSubstring(s);
        return s.Substring(start, length);
    }
}
