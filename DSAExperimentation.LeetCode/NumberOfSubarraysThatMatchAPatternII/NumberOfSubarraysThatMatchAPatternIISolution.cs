using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.NumberOfSubarraysThatMatchAPatternII;

// LeetCode 3036. Number of Subarrays That Match a Pattern II: identical
// mechanics to LC 3034 (Part I) - count the (m+1)-length windows of nums
// whose consecutive-element signs equal pattern. See
// NumberOfSubarraysThatMatchAPatternISolution for the full derivation - this
// file restates it because §17.3 keeps every problem folder self-contained
// rather than reaching across into another problem's solution class.
//
// The only thing that changes here is scale: n reaches 10^6, so the O(n*m)
// brute-force scan (still included below, still what the composed strategy
// has to beat, and still correct at any scale) is no longer viable at this
// problem's own bound - only the O(n + m) strategy is. This file reaches for
// Algorithms.StringMatching.ZFunction rather than Part I's
// PrefixFunctionSearch, proving a second string-matching primitive is
// equally sufficient for the same reduction.
internal static class NumberOfSubarraysThatMatchAPatternIISolution
{
    // The textbook scan, unchanged from Part I: for every candidate start,
    // walk pattern directly against nums, comparing Math.Sign of each
    // consecutive difference and bailing on the first mismatch. O(n*m) - the
    // arm the ZFunction strategy below has to beat, and why it is only
    // benchmarked at a fraction of this problem's own 10^6 bound.
    public static int CountMatchesByBruteForce(int[] nums, int[] pattern)
    {
        var n = nums.Length;
        var m = pattern.Length;
        var count = 0;

        for (var i = 0; i + m < n; i++)
        {
            if (MatchesAt(nums, pattern, i))
            {
                count++;
            }
        }

        return count;
    }

    private static bool MatchesAt(int[] nums, int[] pattern, int start)
    {
        for (var j = 0; j < pattern.Length; j++)
        {
            if (Math.Sign(nums[start + j + 1] - nums[start + j]) != pattern[j])
            {
                return false;
            }
        }

        return true;
    }

    // Encodes nums' consecutive signs and pattern into two short texts over a
    // 3-symbol alphabet, then reduces the whole problem to "how many times
    // does pattern-text occur in nums-text" - exactly what
    // Algorithms.StringMatching.ZFunction answers in O(n + m), the strategy
    // this problem's 10^6 bound actually requires.
    public static int CountMatchesByZFunction(int[] nums, int[] pattern) =>
        ZFunction.FindAll(EncodeDiffs(nums), EncodePattern(pattern)).Count;

    private static string EncodeDiffs(int[] nums)
    {
        var text = new char[nums.Length - 1];

        for (var i = 0; i < text.Length; i++)
        {
            text[i] = EncodeSign(Math.Sign(nums[i + 1] - nums[i]));
        }

        return new string(text);
    }

    private static string EncodePattern(int[] pattern)
    {
        var text = new char[pattern.Length];

        for (var i = 0; i < pattern.Length; i++)
        {
            text[i] = EncodeSign(pattern[i]);
        }

        return new string(text);
    }

    // sign is always -1, 0 or 1 here (either a pattern entry or Math.Sign of
    // a consecutive difference), so +1 lands on '0'/'1'/'2' - three ordinary,
    // directly comparable chars, with no other symbol ever introduced to
    // collide with them.
    private static char EncodeSign(int sign) => (char)('1' + sign);
}
