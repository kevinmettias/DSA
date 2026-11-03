using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.Solutions.MinimumPartitionScore;

// LeetCode 3826. Minimum Partition Score: split nums into exactly groupCount
// contiguous subarrays, minimizing the sum over every subarray of
// sumArr * (sumArr + 1) / 2.
//
// State (Index, GroupsUsed) = "min score to finish partitioning nums[Index..]
// into the remaining groupCount - GroupsUsed groups" - the same shape
// MinimumSumOfValuesByDividingArraySolution's (Index, GroupIndex) state uses,
// with a triangular-number cost in place of a running-AND target. A
// partition into exactly groupCount groups always exists for
// 1 <= groupCount <= nums.Length,
// so unlike that problem this recurrence never needs a "no valid split"
// sentinel. Both strategies run the same step and differ only in how
// repeated states get cached - a Dictionary hand-threaded through a plain
// recursion, or this repo's own Memoizer - the same contrast that file
// draws. The baseline writes its recursion out directly rather than through
// Memoizer's IRecurrence contract, because a baseline uses no library
// implementation (ARCHITECTURE 17.5).
internal static class MinimumPartitionScoreSolution
{
    // The textbook form: recursion over a hand-rolled Dictionary memo table,
    // its lookup/store written out at every call site. The arm the repo's
    // own Memoizer-based strategy below has to beat.
    public static long MinPartitionScoreByDictionaryMemo(int[] nums, int groupCount)
    {
        var prefixSums = BuildPrefixSums(nums);
        var run = new DictionaryMemoRun(prefixSums, groupCount, []);

        return DictionaryMemoScore((0, 0), run);
    }

    // Same recurrence, routed through this repo's own Memoizer so each
    // distinct (Index, GroupsUsed) state is solved once without hand-writing
    // the cache lookup/store around every call.
    public static long MinPartitionScoreByMemoizedPartition(int[] nums, int groupCount) =>
        Memoizer.Memoize((0, 0), new ScoreFromPartitionStart(BuildPrefixSums(nums), groupCount));

    private static long PartitionStep(
        (int Index, int GroupsUsed) state,
        long[] prefixSums,
        int groupCount,
        IRecurrence<(int Index, int GroupsUsed), long> solveRest)
    {
        var (index, groupsUsed) = state;
        var elementCount = prefixSums.Length - 1;
        var remainingGroups = groupCount - groupsUsed;

        if (remainingGroups == 1)
        {
            return SubarrayValue(prefixSums, index, elementCount);
        }

        // Every remaining group needs at least one element, so this group
        // may claim at most elementCount - index - (remainingGroups - 1) of them.
        var lastEnd = elementCount - (remainingGroups - 1);

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

    // The hand-rolled memo lookup and store around one (Index, GroupsUsed) state.
    private static long DictionaryMemoScore((int Index, int GroupsUsed) state, DictionaryMemoRun run)
    {
        if (run.Memo.TryGetValue(state, out var cached))
        {
            return cached;
        }

        var result = DictionaryMemoStep(state, run);
        run.Memo[state] = result;
        return result;
    }

    // The same step as PartitionStep, the rest of the partition asked of the
    // Dictionary-memoized recursion directly.
    private static long DictionaryMemoStep((int Index, int GroupsUsed) state, DictionaryMemoRun run)
    {
        var (index, groupsUsed) = state;
        var elementCount = run.PrefixSums.Length - 1;
        var remainingGroups = run.GroupCount - groupsUsed;

        if (remainingGroups == 1)
        {
            return SubarrayValue(run.PrefixSums, index, elementCount);
        }

        // Every remaining group needs at least one element, so this group
        // may claim at most elementCount - index - (remainingGroups - 1) of them.
        var lastEnd = elementCount - (remainingGroups - 1);

        return DictionaryMemoBestOverGroupEnds(run, index, lastEnd, groupsUsed + 1);
    }

    // The same choice of group ends as BestScoreOverGroupEnds, each remainder
    // answered by the Dictionary-memoized recursion.
    private static long DictionaryMemoBestOverGroupEnds(
        DictionaryMemoRun run, int index, int lastEnd, int nextGroupsUsed)
    {
        var best = long.MaxValue;

        for (var end = index + 1; end <= lastEnd; end++)
        {
            var candidate = SubarrayValue(run.PrefixSums, index, end)
                + DictionaryMemoScore((end, nextGroupsUsed), run);

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

    // What the hand-rolled memo run carries through every call: the fixed inputs and
    // the table its lookup and store read and write.
    private readonly record struct DictionaryMemoRun(
        long[] PrefixSums, int GroupCount, Dictionary<(int Index, int GroupsUsed), long> Memo);

    // The next group's start index and how many groups are already used are the whole
    // state: everything else the step reads is fixed for the call and lives here.
    private sealed class ScoreFromPartitionStart(long[] prefixSums, int groupCount)
        : IRecurrence<(int Index, int GroupsUsed), long>
    {
        public long Replay((int Index, int GroupsUsed) state, IRecurrence<(int Index, int GroupsUsed), long> rest)
            => PartitionStep(state, prefixSums, groupCount, rest);
    }

}
