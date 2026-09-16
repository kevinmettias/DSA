using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.MinimumTimeToRevertWordToInitialStateI;

// LeetCode 3029. Minimum Time to Revert Word to Initial State I: each second
// drops word's first chunkSize characters and appends chunkSize characters of
// the caller's choosing, so word reverts to itself after t seconds precisely
// when the t*chunkSize-character prefix already dropped can be exactly refilled
// - i.e. when word's suffix starting at index t*chunkSize equals word's own
// prefix of that same length (whatever character were appended earlier are gone
// by then, so only the surviving suffix constrains the refill). Once
// t*chunkSize >= word.Length,
// nothing survives to constrain anything, so the whole original word can
// just be appended back and t is always an answer.
//
// word.Length <= 50 here, so the O(n^2/k) baseline that compares those two
// substrings character-by-character for every candidate t is already fast
// enough - but the O(n) ZFunction strategy is exactly what a much longer word
// (LC 3031's own bound is up to 10^6) would actually need, so it is proven
// here too at this smaller scale, the same way MaximumStrongPairXORI still
// carries the bucket strategy its own bound doesn't strictly require.
internal static class MinimumTimeToRevertWordToInitialStateISolution
{
    // The textbook scan: try t = 1, 2, ... and compare the two candidate
    // substrings directly. Correct at any scale, and the arm the ZFunction
    // strategy below has to beat.
    public static int MinTimeByBruteForce(string word, int chunkSize)
    {
        var n = word.Length;

        for (var t = 1; ; t++)
        {
            var shift = t * chunkSize;

            if (shift >= n || IsSuffixMatchingPrefix(word, shift, n))
            {
                return t;
            }
        }
    }

    // word[shift:] equals word's own prefix of that same length - i.e. whatever
    // the drops have taken so far can be exactly refilled. Only reached while
    // shift < wordLength (the caller short-circuits past it otherwise), so both
    // slices are always in range here.
    private static bool IsSuffixMatchingPrefix(string word, int shift, int wordLength)
    {
        var dropped = word.AsSpan(shift);
        var prefix = word.AsSpan(0, wordLength - shift);

        return dropped.SequenceEqual(prefix);
    }

    // Algorithms.StringMatching.ZFunction.Compute(word)[shift] is the longest
    // common prefix of word and word[shift:] - exactly "does the suffix at
    // shift match the prefix of the same length", for every shift at once in
    // one O(n) pass, so the loop over t only has to look each answer up
    // rather than re-comparing substrings.
    public static int MinTimeByZFunction(string word, int chunkSize)
    {
        var n = word.Length;
        var z = ZFunction.Compute(word);

        for (var t = 1; ; t++)
        {
            var shift = t * chunkSize;

            if (shift >= n || z[shift] >= n - shift)
            {
                return t;
            }
        }
    }
}
