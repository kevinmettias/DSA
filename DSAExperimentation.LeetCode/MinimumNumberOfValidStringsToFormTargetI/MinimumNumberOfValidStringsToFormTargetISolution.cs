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
// each other and the benchmark harness can time them side by side. The brute-force
// reach scan is this class's own, LC 3291's bound being what keeps it tractable;
// the ZFunction reach scan is MinimumNumberOfValidStringsToFormTargetIISolution's,
// LC 3292's 5*10^4 bound being what requires it, and this class calls through.
internal static class MinimumNumberOfValidStringsToFormTargetISolution
{
    // Textbook O(target.Length * sum(words[i].Length)): for every start position,
    // compare against every word character by character. Deliberately plain BCL
    // string indexing, no repo primitive - the arm the ZFunction strategy below
    // has to beat.
    public static int MinValidStringsByBruteForce(string[] words, string target)
    {
        var reach = ReachByBruteForce(words, target);

        return MinJumps(reach);
    }

    private static int[] ReachByBruteForce(string[] words, string target)
    {
        var n = target.Length;
        var reach = new int[n];

        for (var i = 0; i < n; i++)
        {
            reach[i] = LongestValidFrom(words, target, i);
        }

        return reach;
    }

    // The longest valid string that can start at `start`: the longest common prefix
    // of target[start:] with any one word. `limit` keeps the character comparison
    // inside target, so the scan can never read past its end - a word longer than
    // the remainder of target simply stops matching there.
    private static int LongestValidFrom(string[] words, string target, int start)
    {
        var best = 0;

        foreach (var word in words)
        {
            var limit = Math.Min(word.Length, target.Length - start);
            var matched = 0;

            while (matched < limit && word[matched] == target[start + matched])
            {
                matched++;
            }

            best = Math.Max(best, matched);
        }

        return best;
    }

    // Composed: for each word, DSAExperimentation.Algorithms.StringMatching.ZFunction.Compute
    // over word + a sentinel outside the lowercase alphabet + target gives, in one
    // O(word.Length + target.Length) pass, the longest common prefix of word and
    // target[i:] for every i at once - the sentinel can never itself match a
    // lowercase target character, so the match can never run past word.Length and
    // "read past" it into target's own content. Folding the max of that array over
    // every word yields reach[] directly. LC 3292's own 5*10^4 bound is what
    // requires that reach scan, which makes its class the one implementation of
    // it; this arm calls through. Nothing narrows - both parts answer an int over
    // the same signature.
    public static int MinValidStringsByZFunctionAcrossWords(string[] words, string target) =>
        MinimumNumberOfValidStringsToFormTargetIISolution.MinValidStringsByZFunctionAcrossWords(words, target);

    // Jump Game II over reach[]: currentEnd is the farthest position reachable
    // using the jump count committed so far; farthest is the farthest position
    // reachable using one more jump from anywhere already visited. Advancing
    // currentEnd only when the scan reaches it (rather than after every step)
    // is what keeps this O(target.Length) instead of O(target.Length^2).
    private static int MinJumps(int[] reach)
    {
        var n = reach.Length;
        var jumps = 0;
        var currentEnd = 0;
        var farthest = 0;

        for (var i = 0; i < n; i++)
        {
            if (i > farthest)
            {
                return LeetCodeAnswer.None;
            }

            farthest = Math.Max(farthest, i + reach[i]);
            (jumps, currentEnd, var answer) = CloseJumpWindowIfAtEnd((i, currentEnd), farthest, jumps, n);

            if (answer.HasValue)
            {
                return answer.Value;
            }
        }

        return currentEnd >= n ? jumps : LeetCodeAnswer.None;
    }

    // A jump window closes when the scan reaches currentEnd: one more jump is spent
    // and the window becomes everything one further jump can reach. A window that
    // reaches nothing new means target can never be formed, and covering n means it
    // has been - `Answer` carries what to return in either case (None, or the jump
    // count) and is null while the sweep must carry on. An index short of the end
    // leaves the cursor untouched.
    private static (int Jumps, int CurrentEnd, int? Answer) CloseJumpWindowIfAtEnd(
        (int Index, int CurrentEnd) window,
        int farthest,
        int jumps,
        int n)
    {
        if (window.Index != window.CurrentEnd)
        {
            return (jumps, window.CurrentEnd, null);
        }

        if (farthest == window.CurrentEnd)
        {
            return (jumps, window.CurrentEnd, LeetCodeAnswer.None);
        }

        var nextJumps = jumps + 1;
        var nextEnd = farthest;

        return (nextJumps, nextEnd, nextEnd >= n ? nextJumps : null);
    }
}
