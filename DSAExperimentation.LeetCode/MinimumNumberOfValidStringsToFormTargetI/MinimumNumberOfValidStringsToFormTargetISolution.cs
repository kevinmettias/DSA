using DSAExperimentation.LeetCode.MinimumNumberOfValidStringsToFormTargetII;

namespace DSAExperimentation.LeetCode.MinimumNumberOfValidStringsToFormTargetI;

// LeetCode 3291. Minimum Number of Valid Strings to Form Target I: a string is
// "valid" if it is a prefix of some word in `words`. Concatenate as few valid
// strings as possible to build `target`, or report it is impossible.
//
// For each start position i in target, let reach[i] be the length of the longest
// valid string that could start there - the longest common prefix, over every
// word, between that word and target[i:]. A single valid string can then jump
// from position i to anywhere in (i, i + reach[i]], which is exactly the classic
// Jump Game II shape: the answer is the minimum number of such jumps from 0 to
// target.Length, or -1 if target.Length itself is unreachable.
//
// Both strategies answer the same question with the same signature - they differ
// only in how reach[] is computed - so the test harness can assert them against
// each other and the benchmark harness can time them side by side. LC 3292 asks
// the same question at a 5*10^4 bound, so its class owns both arms (ARCHITECTURE
// 17.3) and this class calls through; both parts answer an int, so nothing narrows.
internal static class MinimumNumberOfValidStringsToFormTargetISolution
{
    // Textbook O(target.Length * sum(words[i].Length)): for every start position,
    // compare against every word character by character - the arm the ZFunction
    // strategy has to beat, and tractable at this problem's bound.
    public static int MinValidStringsByBruteForce(string[] words, string target) =>
        MinimumNumberOfValidStringsToFormTargetIISolution.MinValidStringsByBruteForce(words, target);

    // One Z-function pass per word over word + target yields reach[] directly.
    public static int MinValidStringsByZFunctionAcrossWords(string[] words, string target) =>
        MinimumNumberOfValidStringsToFormTargetIISolution.MinValidStringsByZFunctionAcrossWords(words, target);
}
