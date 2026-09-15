using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.PalindromicSubstrings;

// LeetCode 647. Palindromic Substrings: how many contiguous substrings of the input
// read the same forwards and backwards.
//
// The baseline is the textbook O(n^2) expand-around-every-center walk, counting
// every successful expansion instead of tracking only the longest one -
// LongestPalindromicSubstringSolution's ExpandFrom, read as a count instead of a
// (Start, Length) pair. The composed strategy is this repo's own O(n) Manacher
// primitive: ComputeOddRadii(text)[i] = k means k nested odd-length palindromes are
// centered at i (lengths 1, 3, .., 2k-1), and ComputeEvenRadii(text)[i] = k means k
// nested even-length palindromes are centered between i-1 and i, so summing every
// radius counts every palindromic substring in one pass - the same primitive
// LongestPalindromicSubstringSolution reuses, read differently: that solution takes
// the max radius, this one takes the sum.
internal static class PalindromicSubstringsSolution
{
    public static int CountSubstringsByExpandAroundCenter(string s)
    {
        var count = 0;

        for (var center = 0; center < s.Length; center++)
        {
            count += CountExpansionsFrom(s, center, center);
            count += CountExpansionsFrom(s, center, center + 1);
        }

        return count;
    }

    private static int CountExpansionsFrom(string s, int left, int right)
    {
        var count = 0;

        while (ExpandsFurther(s, left, right))
        {
            count++;
            left--;
            right++;
        }

        return count;
    }

    // The expansion can continue: both indices are still inside the text, and the
    // characters they point at match.
    private static bool ExpandsFurther(string s, int left, int right)
        => left >= 0 && right < s.Length && s[left] == s[right];

    public static int CountSubstringsByManacher(string s)
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
