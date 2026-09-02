using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.LeetCode.MinimumNumberOfValidStringsToFormTargetII;

// LeetCode 3292. Minimum Number of Valid Strings to Form Target II: identical
// mechanics to LC 3291 (Part I) - a string is "valid" if it is a prefix of some
// word in `words`, and the task is the fewest valid strings whose concatenation
// builds `target`. See MinimumNumberOfValidStringsToFormTargetISolution for the
// full derivation - this file restates it because §17.3 keeps every problem
// folder self-contained rather than reaching across into another problem's
// solution class.
//
// The only thing that changes here is scale: target.Length and words[i].Length
// both reach 5*10^4 (10x Part I's bound), so the O(target.Length * sum(words[i].Length))
// brute-force reach scan (still included below, still what the composed strategy
// has to beat, and still correct at any scale) is no longer viable at this
// problem's own bound - only the O(numWords * target.Length + sum(words[i].Length))
// ZFunction strategy is, and words.length <= 100 keeps that factor small.
internal static class MinimumNumberOfValidStringsToFormTargetIISolution
{
    // Textbook O(target.Length * sum(words[i].Length)): for every start position,
    // compare against every word character by character. Deliberately plain BCL
    // string indexing, no repo primitive - the arm the ZFunction strategy below
    // has to beat, and why it is only benchmarked at a fraction of this problem's
    // own 5*10^4 bound.
    public static int MinValidStringsByBruteForce(string[] words, string target)
        => MinJumps(ReachByBruteForce(words, target));

    private static int[] ReachByBruteForce(string[] words, string target)
    {
        var n = target.Length;
        var reach = new int[n];

        for (var i = 0; i < n; i++)
        {
            var best = 0;

            foreach (var word in words)
            {
                var limit = Math.Min(word.Length, n - i);
                var matched = 0;

                while (matched < limit && word[matched] == target[i + matched])
                {
                    matched++;
                }

                best = Math.Max(best, matched);
            }

            reach[i] = best;
        }

        return reach;
    }

    // Composed: for each word, DSAExperimentation.Algorithms.StringMatching.ZFunction.Compute
    // over word + a sentinel outside the lowercase alphabet + target gives, in one
    // O(word.Length + target.Length) pass, the longest common prefix of word and
    // target[i:] for every i at once - the sentinel can never itself match a
    // lowercase target character, so the match can never run past word.Length and
    // "read past" it into target's own content. Folding the max of that array over
    // every word yields reach[] directly - the strategy this problem's 5*10^4
    // bound actually requires.
    public static int MinValidStringsByZFunctionAcrossWords(string[] words, string target)
        => MinJumps(ReachByZFunctionAcrossWords(words, target));

    private static int[] ReachByZFunctionAcrossWords(string[] words, string target)
    {
        var n = target.Length;
        var reach = new int[n];

        foreach (var word in words)
        {
            var combined = string.Concat(word, " ", target);
            var z = ZFunction.Compute(combined);
            var offset = word.Length + 1;

            for (var i = 0; i < n; i++)
            {
                if (z[offset + i] > reach[i])
                {
                    reach[i] = z[offset + i];
                }
            }
        }

        return reach;
    }

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

            if (i == currentEnd)
            {
                if (farthest == currentEnd)
                {
                    return LeetCodeAnswer.None;
                }

                jumps++;
                currentEnd = farthest;

                if (currentEnd >= n)
                {
                    return jumps;
                }
            }
        }

        return currentEnd >= n ? jumps : LeetCodeAnswer.None;
    }
}
