using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.MinimumTimeToRevertWordToInitialStateII;

// LeetCode 3031. Minimum Time to Revert Word to Initial State II: identical
// mechanics to LC 3029 (Part I) - each second drops word's first k characters
// and appends k characters of the caller's choosing, so word reverts to
// itself after t seconds precisely when the t*k-character prefix already
// dropped can be exactly refilled, i.e. when word's suffix starting at index
// t*k equals word's own prefix of that same length (whatever characters were
// appended earlier are gone by then, so only the surviving suffix constrains
// the refill). Once t*k >= word.Length, nothing survives to constrain
// anything, so the whole original word can just be appended back and t is
// always an answer. See MinimumTimeToRevertWordToInitialStateISolution for
// the full derivation - this file restates it because §17.3 keeps every
// problem folder self-contained rather than reaching across into another
// problem's solution class.
//
// The only thing that changes here is scale: word.Length reaches 10^6, so the
// O(n^2/k) brute-force scan (still included below, still what the composed
// strategy has to beat, and still correct at any scale) is no longer viable
// at this problem's own bound - only the O(n) ZFunction strategy is.
internal static class MinimumTimeToRevertWordToInitialStateIISolution
{
    // The textbook scan: try t = 1, 2, ... and compare the two candidate
    // substrings directly. Correct at any scale, but O(n^2/k) worst case -
    // the arm the ZFunction strategy below has to beat, and why it is only
    // benchmarked at a fraction of this problem's own 10^6 bound.
    public static int MinTimeByBruteForce(string word, int k)
    {
        var n = word.Length;

        for (var t = 1; ; t++)
        {
            var shift = t * k;

            if (shift >= n || SuffixMatchesPrefix(word, shift))
            {
                return t;
            }
        }
    }

    // Whether the surviving suffix at `shift` equals word's own prefix of that same
    // length - exactly the refill condition above. Only reached with shift < word's
    // length, so both spans are non-empty.
    private static bool SuffixMatchesPrefix(string word, int shift)
    {
        var suffix = word.AsSpan(shift);
        var prefix = word.AsSpan(0, word.Length - shift);

        return suffix.SequenceEqual(prefix);
    }

    // Algorithms.StringMatching.ZFunction.Compute(word)[shift] is the longest
    // common prefix of word and word[shift:] - exactly "does the suffix at
    // shift match the prefix of the same length", for every shift at once in
    // one O(n) pass, so the loop over t only has to look each answer up
    // rather than re-comparing substrings. This is the strategy this
    // problem's 10^6 bound actually requires.
    public static int MinTimeByZFunction(string word, int k)
    {
        var n = word.Length;
        var z = ZFunction.Compute(word);

        for (var t = 1; ; t++)
        {
            var shift = t * k;

            if (shift >= n || z[shift] >= n - shift)
            {
                return t;
            }
        }
    }
}
