using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindMaximumNonDecreasingArrayLength;

// LeetCode 2945. Find Maximum Non-decreasing Array Length: repeatedly replacing a
// subarray with its sum is exactly choosing a partition of nums into contiguous
// blocks whose sums are non-decreasing, and the question is the largest number of
// blocks any partition can have.
//
// dp[i] is the most blocks a partition of nums[0..i-1] can have, and last[i] is
// the *smallest* possible sum of that partition's final block among every
// partition achieving dp[i] blocks (greedy-exchange argument: a smaller last
// block only ever makes it easier to extend later, never harder). Extending the
// prefix ending at cut point j into i is legal exactly when
// prefix[i] - prefix[j] >= last[j], i.e. prefix[j] + last[j] <= prefix[i], so
// among the valid j the one maximizing dp[j] and, within ties, minimizing that
// key, is the best predecessor.
//
// Both strategies compute the exact same dp/last recurrence; they differ only in
// how the best predecessor j is found.
internal static class FindMaximumNonDecreasingArrayLengthSolution
{
    // The textbook O(n^2) form: scan every earlier cut point j for each i instead
    // of narrowing the search. The arm the monotonic-stack strategy below has to
    // beat.
    public static int FindMaxLengthByBruteForceDp(int[] nums)
    {
        var n = nums.Length;
        var prefix = BuildPrefixSums(nums);
        var dp = new int[n + 1];
        var last = new long[n + 1];

        for (var i = 1; i <= n; i++)
        {
            var bestDp = -1;
            var bestLast = 0L;

            for (var j = 0; j < i; j++)
            {
                var candidateLast = prefix[i] - prefix[j];
                if (candidateLast < last[j])
                {
                    continue;
                }

                if (dp[j] + 1 > bestDp || (dp[j] + 1 == bestDp && candidateLast < bestLast))
                {
                    bestDp = dp[j] + 1;
                    bestLast = candidateLast;
                }
            }

            dp[i] = bestDp;
            last[i] = bestLast;
        }

        return dp[n];
    }

    // Same recurrence, but the best predecessor is found by keeping only the
    // Pareto-optimal cut points seen so far - a candidate j is dropped the moment
    // a later candidate matches or beats it on both dp and key - in this repo's
    // own DynamicArray<T> used as an indexable stack. Because dp is non-decreasing
    // and the surviving keys are strictly increasing along the stack, the
    // rightmost surviving key that does not exceed prefix[i] is exactly the best
    // predecessor, found by Searching.BinarySearch.UpperBound over
    // DynamicArraySequence<long>'s view of the key stack instead of an O(i) scan.
    public static int FindMaxLengthByMonotonicStackBinarySearch(int[] nums)
    {
        var n = nums.Length;
        var prefix = BuildPrefixSums(nums);
        var dp = new int[n + 1];
        var last = new long[n + 1];

        var stackCuts = new DynamicArray<int>();
        var stackKeys = new DynamicArray<long>();
        stackCuts.Add(0);
        stackKeys.Add(0L);

        for (var i = 1; i <= n; i++)
        {
            var keys = new DynamicArraySequence<long>(stackKeys);
            var position = BinarySearch.UpperBound<long, DynamicArraySequence<long>>(keys, prefix[i]) - 1;
            var j = stackCuts.Get(position);

            dp[i] = dp[j] + 1;
            last[i] = prefix[i] - prefix[j];
            var key = prefix[i] + last[i];

            while (stackCuts.Count > 0 &&
                   dp[stackCuts.Get(stackCuts.Count - 1)] <= dp[i] &&
                   stackKeys.Get(stackKeys.Count - 1) >= key)
            {
                stackCuts.RemoveAt(stackCuts.Count - 1);
                stackKeys.RemoveAt(stackKeys.Count - 1);
            }

            stackCuts.Add(i);
            stackKeys.Add(key);
        }

        return dp[n];
    }

    private static long[] BuildPrefixSums(int[] nums)
    {
        var prefix = new long[nums.Length + 1];

        for (var i = 1; i <= nums.Length; i++)
        {
            prefix[i] = prefix[i - 1] + nums[i - 1];
        }

        return prefix;
    }
}
