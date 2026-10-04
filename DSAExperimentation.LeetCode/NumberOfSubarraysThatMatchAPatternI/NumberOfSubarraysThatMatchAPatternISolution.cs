using DSAExperimentation.Algorithms.StringMatching;
using DSAExperimentation.LeetCode.NumberOfSubarraysThatMatchAPatternII;

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
//
// LC 3036 asks the same question at n up to 10^6, so its class owns the brute
// force and that sign encoding (ARCHITECTURE 17.3), and this class calls in for
// them. The matcher stays this class's own: Part I searches with the prefix
// function where Part II uses the Z-function, two string-matching primitives
// proving equally sufficient for one reduction.
internal static class NumberOfSubarraysThatMatchAPatternISolution
{
    // The textbook scan: for every candidate start, walk pattern directly against
    // nums, bailing on the first mismatch. O(n*m); the arm the composed strategy
    // below has to beat.
    public static int CountMatchesByBruteForce(int[] nums, int[] pattern) =>
        NumberOfSubarraysThatMatchAPatternIISolution.CountMatchesByBruteForce(nums, pattern);

    // How many times pattern-text occurs in nums-text - exactly what
    // Algorithms.StringMatching.PrefixFunctionSearch answers in O(n + m).
    public static int CountMatchesByPrefixFunctionSearch(int[] nums, int[] pattern) =>
        PrefixFunctionSearch.FindAll(
            NumberOfSubarraysThatMatchAPatternIISolution.EncodeDiffs(nums),
            NumberOfSubarraysThatMatchAPatternIISolution.EncodePattern(pattern)).Count;
}
