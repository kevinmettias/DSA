using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumSumOfValuesByDividingArray;

// LeetCode 3117. Minimum Sum of Values by Dividing Array: split nums into
// exactly andValues.Length contiguous groups, the i-th group's bitwise AND
// equal to andValues[i], minimizing the sum of each group's last element (-1
// if no split works).
//
// State (Index, GroupIndex) = "partition nums[Index..] into the remaining
// andValues[GroupIndex..]". Both strategies share the identical recurrence,
// PartitionStep - a group's running AND only ever clears bits as it grows, so
// it can never climb back to a target it has already dropped below, letting
// PartitionStep stop extending a group early - and differ only in how
// repeated states get cached: a Dictionary hand-threaded through the
// recursion, or this repo's own Memoizer, the same "tuple state through
// Memoizer.Memoize<TState, TResult>" contrast
// MaximumNumberOfOperationsWithTheSameScoreIISolution already draws.
internal static class MinimumSumOfValuesByDividingArraySolution
{
    // The textbook form: recursion over a hand-rolled Dictionary memo table,
    // its lookup/store written out at every call site. The arm the repo's
    // own Memoizer-based strategy below has to beat.
    public static long MinimumValueSumByDictionaryMemo(int[] nums, int[] andValues)
    {
        var run = new DictionaryMemoRun(nums, andValues);

        return run.Replay((0, 0), run) ?? LeetCodeAnswer.None;
    }

    // Same recurrence, routed through this repo's own Memoizer so each
    // distinct (Index, GroupIndex) state is solved once without hand-writing
    // the cache lookup/store around every call.
    public static long MinimumValueSumByMemoizedPartition(int[] nums, int[] andValues)
    {
        var result = Memoizer.Memoize((0, 0), new MinimumSumFromGroupStart(nums, andValues));

        return result ?? LeetCodeAnswer.None;
    }

    // The next group's start index and which andValue it must match are the whole state;
    // the array and the targets stay fixed for the call and live here.
    private sealed class MinimumSumFromGroupStart(int[] nums, int[] andValues)
        : IRecurrence<(int Index, int GroupIndex), long?>
    {
        public long? Replay((int Index, int GroupIndex) state, IRecurrence<(int Index, int GroupIndex), long?> rest)
            => PartitionStep(state, nums, andValues, rest);
    }

    // The hand-rolled memo run: it keeps the table and hands itself to the recurrence as
    // the recursion, exactly as Memoizer's own run does - the lookup and the store are
    // just written out here instead of being supplied by the library.
    private sealed class DictionaryMemoRun(int[] nums, int[] andValues)
        : IRecurrence<(int Index, int GroupIndex), long?>
    {
        private readonly MinimumSumFromGroupStart _step = new(nums, andValues);
        private readonly Dictionary<(int Index, int GroupIndex), long?> _memo = [];

        public long? Replay((int Index, int GroupIndex) state, IRecurrence<(int Index, int GroupIndex), long?> rest)
        {
            if (_memo.TryGetValue(state, out var cached))
            {
                return cached;
            }

            var result = _step.Replay(state, this);
            _memo[state] = result;
            return result;
        }
    }

    // null means "no valid partition from here" - the same nullable-over-
    // sentinel choice ARCHITECTURE.md records for HammingSearch, so this
    // shared step never has to agree on a magic "impossible" value with
    // either caller.
    private static long? PartitionStep(
        (int Index, int GroupIndex) state,
        int[] nums,
        int[] andValues,
        IRecurrence<(int Index, int GroupIndex), long?> solveRest)
    {
        var (index, groupIndex) = state;
        var remainingGroups = andValues.Length - groupIndex;

        if (remainingGroups == 0)
        {
            return index == nums.Length ? 0L : null;
        }

        if (nums.Length - index < remainingGroups)
        {
            return null;
        }

        return BestGroupExtension(state, nums, andValues, solveRest);
    }

    // The cheapest completion of the current group: it starts at `index` holding that
    // element's own value and may stop anywhere from `index` onward.
    private static long? BestGroupExtension(
        (int Index, int GroupIndex) state,
        int[] nums,
        int[] andValues,
        IRecurrence<(int Index, int GroupIndex), long?> solveRest)
    {
        var (index, groupIndex) = state;
        long runningAnd = nums[index];
        long? best = null;

        // The AND only ever clears bits, so once it has dropped below the target it can
        // never climb back - the group stops extending there, in the loop bound itself.
        for (var end = index; end < nums.Length && runningAnd >= andValues[groupIndex]; end++)
        {
            runningAnd = FoldNext(nums, runningAnd, end, index);

            if (runningAnd != andValues[groupIndex])
            {
                continue;
            }

            var tail = solveRest.Replay((end + 1, groupIndex + 1), solveRest);
            best = tail is null ? best : Cheaper(best, nums[end] + tail.Value);
        }

        return best;
    }

    // The first element seeds the group, every later one is folded into the AND.
    private static long FoldNext(int[] nums, long runningAnd, int end, int index)
    {
        if (end == index)
        {
            return runningAnd;
        }

        return runningAnd & nums[end];
    }

    // The cheaper of two valid group ends, either of which may be absent.
    private static long? Cheaper(long? best, long candidate)
    {
        if (best is null || candidate < best)
        {
            return candidate;
        }

        return best;
    }
}
