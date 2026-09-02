using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumPartitionScore;

// LeetCode 3826. Minimum Partition Score: split nums into exactly k
// contiguous subarrays, minimizing the sum over every subarray of
// sumArr * (sumArr + 1) / 2.
//
// State (Index, GroupsUsed) = "min score to finish partitioning nums[Index..]
// into the remaining k - GroupsUsed groups" - the same shape
// MinimumSumOfValuesByDividingArraySolution's (Index, GroupIndex) state uses,
// with a triangular-number cost in place of a running-AND target. A
// partition into exactly k groups always exists for 1 <= k <= nums.Length,
// so unlike that problem this recurrence never needs a "no valid split"
// sentinel. Both strategies share PartitionStep and differ only in how
// repeated states get cached - a Dictionary hand-threaded through the
// recursion, or this repo's own Memoizer - the same contrast that file
// draws.
internal static class MinimumPartitionScoreSolution
{
    // The textbook form: recursion over a hand-rolled Dictionary memo table,
    // its lookup/store written out at every call site. The arm the repo's
    // own Memoizer-based strategy below has to beat.
    public static long MinPartitionScoreByDictionaryMemo(int[] nums, int k)
    {
        var prefixSums = BuildPrefixSums(nums);
        var memo = new Dictionary<(int Index, int GroupsUsed), long>();

        long Recurse((int Index, int GroupsUsed) state)
        {
            if (memo.TryGetValue(state, out var cached))
            {
                return cached;
            }

            var result = PartitionStep(state, prefixSums, k, Recurse);
            memo[state] = result;
            return result;
        }

        return Recurse((0, 0));
    }

    // Same recurrence, routed through this repo's own Memoizer so each
    // distinct (Index, GroupsUsed) state is solved once without hand-writing
    // the cache lookup/store around every call.
    public static long MinPartitionScoreByMemoizedPartition(int[] nums, int k)
    {
        var prefixSums = BuildPrefixSums(nums);

        long Recurrence((int Index, int GroupsUsed) state, Func<(int, int), long> solveRest) =>
            PartitionStep(state, prefixSums, k, solveRest);

        return Memoizer.Memoize<(int Index, int GroupsUsed), long>((0, 0), Recurrence);
    }

    private static long PartitionStep(
        (int Index, int GroupsUsed) state,
        long[] prefixSums,
        int k,
        Func<(int, int), long> solveRest)
    {
        var (index, groupsUsed) = state;
        var n = prefixSums.Length - 1;
        var remainingGroups = k - groupsUsed;

        if (remainingGroups == 1)
        {
            return SubarrayValue(prefixSums, index, n);
        }

        var best = long.MaxValue;

        // Every remaining group needs at least one element, so this group
        // may claim at most n - index - (remainingGroups - 1) of them.
        var lastEnd = n - (remainingGroups - 1);

        for (var end = index + 1; end <= lastEnd; end++)
        {
            var candidate = SubarrayValue(prefixSums, index, end) + solveRest((end, groupsUsed + 1));

            if (candidate < best)
            {
                best = candidate;
            }
        }

        return best;
    }

    private static long SubarrayValue(long[] prefixSums, int start, int end)
    {
        var sum = prefixSums[end] - prefixSums[start];
        return sum * (sum + 1) / 2;
    }

    private static long[] BuildPrefixSums(int[] nums)
    {
        var prefixSums = new long[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            prefixSums[i + 1] = prefixSums[i] + nums[i];
        }

        return prefixSums;
    }
}
