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
    public static int CountSubstringsByExpandAroundCenter(string text)
    {
        var count = 0;

        for (var center = 0; center < text.Length; center++)
        {
            count += CountExpansionsFrom(text, center, center);
            count += CountExpansionsFrom(text, center, center + 1);
        }

        return count;
    }

    private static int CountExpansionsFrom(string text, int left, int right)
    {
        var count = 0;

        while (CanExpandFurther(text, left, right))
        {
            count++;
            left--;
            right++;
        }

        return count;
    }

    // The expansion can continue: both indices are still inside the text, and the
    // characters they point at match.
    private static bool CanExpandFurther(string text, int left, int right)
        => left >= 0 && right < text.Length && text[left] == text[right];

    public static int CountSubstringsByManacher(string text)
    {
        var count = 0;

        foreach (var radius in Manacher.ComputeOddRadii(text))
        {
            count += radius;
        }

        foreach (var radius in Manacher.ComputeEvenRadii(text))
        {
            count += radius;
        }

        return count;
    }
}
