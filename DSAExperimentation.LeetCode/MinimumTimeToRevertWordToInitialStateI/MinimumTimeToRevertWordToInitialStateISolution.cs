using DSAExperimentation.LeetCode.MinimumTimeToRevertWordToInitialStateII;

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
// (LC 3031's own bound is up to 10^6) would actually need. LC 3031 asks the same
// question at that bound, so its class owns both arms (ARCHITECTURE 17.3) and
// this class calls through, proving them at this smaller scale too.
internal static class MinimumTimeToRevertWordToInitialStateISolution
{
    // The textbook scan: try t = 1, 2, ... and compare the two candidate
    // substrings directly - the arm the ZFunction strategy has to beat.
    public static int MinTimeByBruteForce(string word, int chunkSize) =>
        MinimumTimeToRevertWordToInitialStateIISolution.MinTimeByBruteForce(word, chunkSize);

    // One Z-function pass answers "does the suffix at shift match the prefix of the
    // same length" for every shift at once.
    public static int MinTimeByZFunction(string word, int chunkSize) =>
        MinimumTimeToRevertWordToInitialStateIISolution.MinTimeByZFunction(word, chunkSize);
}
