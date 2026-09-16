using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.NumberOfSubarraysThatMatchAPatternI;

// LeetCode 3034. Number of Subarrays That Match a Pattern I: count the
// (m+1)-length windows of nums whose consecutive-element signs equal
// pattern, where pattern[j] == 1/0/-1 means nums[i+j+1] is greater
// than/equal to/less than nums[i+j].
//
// Reducing nums to its own sign sequence turns this into an ordinary
// substring-occurrence count: build a text of nums' consecutive signs and a
// text of pattern's own entries over the same 3-symbol alphabet, and the
// answer is exactly how many times pattern-text occurs in nums-text.
internal static class NumberOfSubarraysThatMatchAPatternISolution
{
    // The textbook scan: for every candidate start, walk pattern directly
    // against nums, comparing Math.Sign of each consecutive difference and
    // bailing on the first mismatch. O(n*m); the arm the composed strategy
    // below has to beat once LC 3036's larger bound rules it out.
    public static int CountMatchesByBruteForce(int[] nums, int[] pattern)
    {
        var n = nums.Length;
        var m = pattern.Length;
        var count = 0;

        for (var i = 0; i + m < n; i++)
        {
            if (HasMatchAt(nums, pattern, i))
            {
                count++;
            }
        }

        return count;
    }

    private static bool HasMatchAt(int[] nums, int[] pattern, int start)
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
    // Algorithms.StringMatching.PrefixFunctionSearch answers in O(n + m).
    // Well within this problem's own bound (n <= 100) already, but it is the
    // same reduction LC 3036's larger bound requires.
    public static int CountMatchesByPrefixFunctionSearch(int[] nums, int[] pattern) =>
        PrefixFunctionSearch.FindAll(EncodeDiffs(nums), EncodePattern(pattern)).Count;

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
