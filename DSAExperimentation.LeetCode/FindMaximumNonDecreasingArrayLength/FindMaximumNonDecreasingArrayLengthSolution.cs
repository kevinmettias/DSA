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
            (dp[i], last[i]) = BestPredecessorOf(i, prefix, dp, last);
        }

        return dp[n];
    }

    // The predecessor cut point i extends best: the one whose block sum
    // prefix[i] - prefix[j] reaches last[j], ranked by block count and then by the
    // smaller block sum. Nothing qualifying leaves the -1 that says "no partition
    // of this prefix exists".
    private static (int BestBlocks, long BestLast) BestPredecessorOf(int i, long[] prefix, int[] dp, long[] last)
    {
        var bestBlocks = -1;
        var bestLast = 0L;

        for (var j = 0; j < i; j++)
        {
            var candidateLast = prefix[i] - prefix[j];

            if (candidateLast < last[j])
            {
                continue;
            }

            if (IsBetterPredecessor(dp[j] + 1, candidateLast, bestBlocks, bestLast))
            {
                bestBlocks = dp[j] + 1;
                bestLast = candidateLast;
            }
        }

        return (bestBlocks, bestLast);
    }

    // A predecessor of equal standing is worth keeping: it is dropped only when the
    // new cut already offers at least as many blocks and no larger a last-block sum.
    private static bool IsBetterPredecessor(int candidateBlocks, long candidateLast, int bestBlocks, long bestLast)
        => candidateBlocks > bestBlocks || (candidateBlocks == bestBlocks && candidateLast < bestLast);

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
            var key = ApplyBestPredecessor(i, (stackCuts, stackKeys), (prefix, dp, last));
            PushCut(i, key, (stackCuts, stackKeys), dp);
        }

        return dp[n];
    }

    // The rightmost surviving key that prefix[i] still reaches is exactly the best
    // predecessor, because dp is non-decreasing and the surviving keys strictly
    // increase along the stack, so one UpperBound lookup replaces the whole scan.
    // Record what that predecessor extends the partition to, and return the key
    // this cut point contributes to the stack.
    private static long ApplyBestPredecessor(
        int i,
        (DynamicArray<int> Cuts, DynamicArray<long> Keys) stack,
        (long[] Prefix, int[] Dp, long[] Last) tables)
    {
        var keys = new DynamicArraySequence<long>(stack.Keys);
        var position = BinarySearch.UpperBound<long, DynamicArraySequence<long>>(keys, tables.Prefix[i]) - 1;
        var j = stack.Cuts.Get(position);

        tables.Dp[i] = tables.Dp[j] + 1;
        tables.Last[i] = tables.Prefix[i] - tables.Prefix[j];

        return tables.Prefix[i] + tables.Last[i];
    }

    // Stack the new cut point, first dropping every surviving cut it dominates.
    private static void PushCut(
        int i, long key, (DynamicArray<int> Cuts, DynamicArray<long> Keys) stack, int[] dp)
    {
        while (IsDominatedByNewCut(stack, dp, i, key))
        {
            stack.Cuts.RemoveAt(stack.Cuts.Count - 1);
            stack.Keys.RemoveAt(stack.Keys.Count - 1);
        }

        stack.Cuts.Add(i);
        stack.Keys.Add(key);
    }

    // The cut point on top of the Pareto stack is dominated once the new one matches
    // or beats it on both counts - no more blocks than dp[i], and a key at least as
    // large - at which point it can never be the best predecessor again.
    private static bool IsDominatedByNewCut(
        (DynamicArray<int> Cuts, DynamicArray<long> Keys) stack,
        int[] dp,
        int i,
        long key)
        => stack.Cuts.Count > 0
            && dp[stack.Cuts.Get(stack.Cuts.Count - 1)] <= dp[i]
            && stack.Keys.Get(stack.Keys.Count - 1) >= key;

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
