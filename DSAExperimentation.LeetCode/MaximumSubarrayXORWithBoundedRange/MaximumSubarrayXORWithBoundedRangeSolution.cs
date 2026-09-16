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
            if (!IsInRange(nums[start], low, high))
            {
                continue;
            }

            var runningXor = 0;

            for (var end = start; end < nums.Length && IsInRange(nums[end], low, high); end++)
            {
                runningXor ^= nums[end];
                best = Math.Max(best, runningXor);
            }
        }

        return best;
    }

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
        var range = (low, high);

        while (start < nums.Length)
        {
            (best, start) = SweepRun(nums, range, best, start);
        }

        return best;
    }

    // One step of the sweep: an out-of-range value is stepped over outright; an
    // in-range one opens a run, whose own best subarray XOR is folded into `best`
    // before the sweep jumps past the run.
    private static (int Best, int Start) SweepRun(
        int[] nums, (int Low, int High) range, int best, int start)
    {
        if (!IsInRange(nums[start], range.Low, range.High))
        {
            return (best, start + 1);
        }

        var end = start;

        while (end < nums.Length && IsInRange(nums[end], range.Low, range.High))
        {
            end++;
        }

        var runBest = MaxSubarrayXorWithinRun(nums, start, end);

        return (Math.Max(best, runBest), end);
    }

    private static int MaxSubarrayXorWithinRun(int[] nums, int start, int end)
    {
        var (trie, prefixValues) = BuildPrefixTrie(nums, start, end);

        return MaxXorAgainstPrefixes(trie, prefixValues);
    }

    // The run's own prefix-XOR values, each inserted into the trie as it is produced.
    private static (BitTrie Trie, List<int> PrefixValues) BuildPrefixTrie(
        int[] nums, int start, int end)
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

        return (trie, prefixValues);
    }

    // The best XOR any one of the run's prefix values can make against the trie.
    private static int MaxXorAgainstPrefixes(BitTrie trie, List<int> prefixValues)
    {
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

    private static bool IsInRange(int value, int low, int high) => value >= low && value <= high;
}
