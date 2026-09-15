using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MakeArrayNonDecreasing;

// LeetCode 3523. Make Array Non-decreasing: repeatedly replace any subarray with
// its own maximum, any number of times, to make nums non-decreasing; return the
// largest array size reachable this way. An operation collapses a contiguous run
// into a single element equal to that run's max, so the question is exactly "what
// is the largest number of contiguous blocks nums can be partitioned into whose
// maxima are themselves non-decreasing left to right".
internal static class MakeArrayNonDecreasingSolution
{
    private const int EmptyPrefixMax = int.MinValue;

    // The textbook DP: dp(i) is the best (block count, that block's max) achievable
    // for the prefix ending at i, tried over every possible start j of the last
    // block. Memoizer turns the natural recursive statement of that recurrence into
    // a real O(n^2) algorithm instead of the exponential one it would be
    // unmemoized - the same "state graph has massive overlap, cache it" role
    // Memoizer already plays for Fibonacci/knapsack-shaped recurrences.
    public static int MaxSizeByPrefixDynamicProgramming(int[] nums)
    {
        var best = Memoizer.Memoize<int, (int Count, int Max)>(nums.Length - 1, new BestSplit(nums));

        return best.Count;
    }

    // One candidate start j for the last block: fold nums[j] into the running block
    // max, drop the split when that block would exceed the best prefix before it, and
    // otherwise keep the split if it beats the incumbent. Both running values travel
    // back to the caller as one tuple since neither is meaningful without the other.
    private static ((int Count, int Max) Best, int BlockMax) ExtendLastBlock(
        int value, (int Count, int Max) prefix, (int Count, int Max) best, int blockMax)
    {
        var extended = Math.Max(blockMax, value);

        if (extended >= prefix.Max)
        {
            var candidate = (Count: prefix.Count + 1, Max: extended);

            if (IsBetterSplit(candidate, best))
            {
                best = candidate;
            }
        }

        return (best, extended);
    }

    // A better split for this prefix: more blocks, or the same count ending in a
    // smaller block max.
    private static bool IsBetterSplit((int Count, int Max) candidate, (int Count, int Max) best)
        => candidate.Count > best.Count
            || (candidate.Count == best.Count && candidate.Max < best.Max);

    // The greedy: an element either extends the current block (it's <= the running
    // max, so folding it in changes nothing) or starts a fresh one-element block (it
    // is >= the running max, so it can only help to commit it immediately - deferring
    // never creates an option that starting fresh doesn't already have).
    public static int MaxSizeByGreedyScan(int[] nums)
    {
        var count = 0;
        var runningMax = EmptyPrefixMax;

        foreach (var value in nums)
        {
            if (value >= runningMax)
            {
                count++;
                runningMax = value;
            }
        }

        return count;
    }

    // The prefix rule, named: dp(i) is the best (block count, closing block max) for the
    // prefix ending at i, tried over every possible start j of the last block.
    private sealed class BestSplit(int[] nums) : IRecurrence<int, (int Count, int Max)>
    {
        /// <inheritdoc/>
        public (int Count, int Max) Replay(int state, IRecurrence<int, (int Count, int Max)> rest)
        {
            if (state < 0)
            {
                return (0, EmptyPrefixMax);
            }

            var best = (Count: 0, Max: EmptyPrefixMax);
            var blockMax = int.MinValue;

            for (var j = state; j >= 0; j--)
            {
                var prefix = rest.Replay(j - 1, rest);

                (best, blockMax) = ExtendLastBlock(nums[j], prefix, best, blockMax);
            }

            return best;
        }
    }
}
