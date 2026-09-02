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
        var best = Memoizer.Memoize<int, (int Count, int Max)>(
            nums.Length - 1,
            (i, dp) => BestSplit(nums, i, dp));

        return best.Count;
    }

    private static (int Count, int Max) BestSplit(int[] nums, int i, Func<int, (int Count, int Max)> dp)
    {
        if (i < 0)
        {
            return (0, EmptyPrefixMax);
        }

        var best = (Count: 0, Max: EmptyPrefixMax);
        var blockMax = int.MinValue;

        for (var j = i; j >= 0; j--)
        {
            blockMax = Math.Max(blockMax, nums[j]);
            var prefix = dp(j - 1);

            if (blockMax < prefix.Max)
            {
                continue;
            }

            var candidate = (Count: prefix.Count + 1, Max: blockMax);

            if (candidate.Count > best.Count || (candidate.Count == best.Count && candidate.Max < best.Max))
            {
                best = candidate;
            }
        }

        return best;
    }

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
}
