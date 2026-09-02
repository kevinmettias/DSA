using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.MaximumSubarrayXORWithBoundedRange;

// A subarray is "valid" here exactly when every one of its elements lies in
// [low, high]; among every valid, non-empty contiguous subarray, report the
// maximum XOR of its elements (0 if no element of nums lies in [low, high] at
// all, so no valid subarray exists).
internal static class MaximumSubarrayXORWithBoundedRangeSolution
{
    // The textbook O(n^2) scan: for every start index inside range, extend the
    // running XOR one element at a time until a value falls outside
    // [low, high], tracking the best seen along the way - the arm the trie
    // strategy below has to beat.
    public static int MaxSubarrayXorByBruteForce(int[] nums, int low, int high)
    {
        var best = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            if (!InRange(nums[start], low, high))
            {
                continue;
            }

            var runningXor = 0;

            for (var end = start; end < nums.Length && InRange(nums[end], low, high); end++)
            {
                runningXor ^= nums[end];
                best = Math.Max(best, runningXor);
            }
        }

        return best;
    }

    private static bool InRange(int value, int low, int high) => value >= low && value <= high;

    // Composed: split nums into its maximal [low, high]-valid runs - an
    // out-of-range value can never appear inside a valid subarray, so it always
    // ends one run and starts the search for the next. Within one run, a
    // subarray's XOR is exactly prefixXor[b] XOR prefixXor[a] for two of that
    // run's own prefix-XOR values - the same reduction
    // MaximumXOROfTwoNumbersInAnArray makes for a whole array - so inserting
    // every prefix value into one BitTrie and querying each against it finds
    // the run's own best subarray XOR, the same insert-all-then-query-all
    // composition MaximumStrongPairXORIISolution.MaxXorWithinBucket uses per
    // bucket.
    public static int MaxSubarrayXorByBitTrieSegments(int[] nums, int low, int high)
    {
        var best = 0;
        var start = 0;

        while (start < nums.Length)
        {
            if (!InRange(nums[start], low, high))
            {
                start++;
                continue;
            }

            var end = start;

            while (end < nums.Length && InRange(nums[end], low, high))
            {
                end++;
            }

            best = Math.Max(best, MaxSubarrayXorWithinRun(nums, start, end));
            start = end;
        }

        return best;
    }

    private static int MaxSubarrayXorWithinRun(int[] nums, int start, int end)
    {
        var trie = new BitTrie();
        var prefixValues = new List<int> { 0 };
        var prefixXor = 0;
        trie.Insert(prefixXor);

        for (var i = start; i < end; i++)
        {
            prefixXor ^= nums[i];
            trie.Insert(prefixXor);
            prefixValues.Add(prefixXor);
        }

        var best = 0;

        foreach (var value in prefixValues)
        {
            if (trie.TryMaxXor(value, out var candidate))
            {
                best = Math.Max(best, candidate);
            }
        }

        return best;
    }
}
