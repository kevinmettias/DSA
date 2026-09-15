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
        var run = new DictionaryMemoRun(BuildPrefixSums(nums), k);

        return run.Replay((0, 0), run);
    }

    // Same recurrence, routed through this repo's own Memoizer so each
    // distinct (Index, GroupsUsed) state is solved once without hand-writing
    // the cache lookup/store around every call.
    public static long MinPartitionScoreByMemoizedPartition(int[] nums, int k) =>
        Memoizer.Memoize((0, 0), new ScoreFromPartitionStart(BuildPrefixSums(nums), k));

    // The next group's start index and how many groups are already used are the whole
    // state: everything else the step reads is fixed for the call and lives here.
    private sealed class ScoreFromPartitionStart(long[] prefixSums, int k)
        : IRecurrence<(int Index, int GroupsUsed), long>
    {
        public long Replay((int Index, int GroupsUsed) state, IRecurrence<(int Index, int GroupsUsed), long> rest)
            => PartitionStep(state, prefixSums, k, rest);
    }

    // The hand-rolled memo run: it keeps the table and hands itself to the recurrence as
    // the recursion, exactly as Memoizer's own run does - the lookup and the store are
    // just written out here instead of being supplied by the library.
    private sealed class DictionaryMemoRun(long[] prefixSums, int k)
        : IRecurrence<(int Index, int GroupsUsed), long>
    {
        private readonly ScoreFromPartitionStart step = new(prefixSums, k);
        private readonly Dictionary<(int Index, int GroupsUsed), long> memo = [];

        public long Replay((int Index, int GroupsUsed) state, IRecurrence<(int Index, int GroupsUsed), long> rest)
        {
            if (memo.TryGetValue(state, out var cached))
            {
                return cached;
            }

            var result = step.Replay(state, this);
            memo[state] = result;
            return result;
        }
    }

    private static long PartitionStep(
        (int Index, int GroupsUsed) state,
        long[] prefixSums,
        int k,
        IRecurrence<(int Index, int GroupsUsed), long> solveRest)
    {
        var (index, groupsUsed) = state;
        var n = prefixSums.Length - 1;
        var remainingGroups = k - groupsUsed;

        if (remainingGroups == 1)
        {
            return SubarrayValue(prefixSums, index, n);
        }

        // Every remaining group needs at least one element, so this group
        // may claim at most n - index - (remainingGroups - 1) of them.
        var lastEnd = n - (remainingGroups - 1);

        return BestScoreOverGroupEnds(prefixSums, index, lastEnd, (groupsUsed + 1, solveRest));
    }

    // Tries every legal choice of where this group ends - every `end` that still
    // leaves at least one element for each group yet to come - and keeps the
    // smallest resulting score.
    private static long BestScoreOverGroupEnds(
        long[] prefixSums,
        int index,
        int lastEnd,
        (int NextGroupsUsed, IRecurrence<(int Index, int GroupsUsed), long> SolveRest) next)
    {
        var best = long.MaxValue;

        for (var end = index + 1; end <= lastEnd; end++)
        {
            var candidate = SubarrayValue(prefixSums, index, end)
                + next.SolveRest.Replay((end, next.NextGroupsUsed), next.SolveRest);

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
