using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumSumOfValuesByDividingArray;

// LeetCode 3117. Minimum Sum of Values by Dividing Array: split nums into
// exactly andValues.Length contiguous groups, the i-th group's bitwise AND
// equal to andValues[i], minimizing the sum of each group's last element (-1
// if no split works).
//
// State (Index, GroupIndex) = "partition nums[Index..] into the remaining
// andValues[GroupIndex..]". Both strategies run the identical recurrence -
// a group's running AND only ever clears bits as it grows, so it can never
// climb back to a target it has already dropped below, letting the step stop
// extending a group early - and differ only in how repeated states get
// cached: a Dictionary hand-threaded through a plain recursion, or this
// repo's own Memoizer, the same "tuple state through
// Memoizer.Memoize<TState, TResult>" contrast
// MaximumNumberOfOperationsWithTheSameScoreIISolution already draws. The
// baseline writes its recursion out directly rather than through Memoizer's
// IRecurrence contract, because a baseline reaches no library type
// (ARCHITECTURE 17.5).
internal static class MinimumSumOfValuesByDividingArraySolution
{
    // The textbook form: recursion over a hand-rolled Dictionary memo table,
    // its lookup/store written out at every call site. The arm the repo's
    // own Memoizer-based strategy below has to beat.
    public static long MinimumValueSumByDictionaryMemo(int[] nums, int[] andValues)
    {
        var run = new DictionaryMemoRun(nums, andValues, []);

        return DictionaryMemoSum((0, 0), run) ?? LeetCodeAnswer.None;
    }

    // Same recurrence, routed through this repo's own Memoizer so each
    // distinct (Index, GroupIndex) state is solved once without hand-writing
    // the cache lookup/store around every call.
    public static long MinimumValueSumByMemoizedPartition(int[] nums, int[] andValues)
    {
        var result = Memoizer.Memoize((0, 0), new MinimumSumFromGroupStart(nums, andValues));

        return result ?? LeetCodeAnswer.None;
    }

    // null means "no valid partition from here" - the same nullable-over-
    // sentinel choice ARCHITECTURE.md records for HammingSearch, so this
    // step never has to agree on a magic "impossible" value with its
    // callers.
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

    // The hand-rolled memo lookup and store around one (Index, GroupIndex) state.
    private static long? DictionaryMemoSum((int Index, int GroupIndex) state, DictionaryMemoRun run)
    {
        if (run.Memo.TryGetValue(state, out var cached))
        {
            return cached;
        }

        var result = DictionaryMemoStep(state, run);
        run.Memo[state] = result;
        return result;
    }

    // The same step as PartitionStep: null means "no valid partition from here".
    private static long? DictionaryMemoStep((int Index, int GroupIndex) state, DictionaryMemoRun run)
    {
        var (index, groupIndex) = state;
        var remainingGroups = run.AndValues.Length - groupIndex;

        if (remainingGroups == 0)
        {
            return index == run.Nums.Length ? 0L : null;
        }

        if (run.Nums.Length - index < remainingGroups)
        {
            return null;
        }

        return DictionaryMemoGroupExtension(state, run);
    }

    // The same cheapest completion of the current group as BestGroupExtension, each
    // remainder answered by the Dictionary-memoized recursion.
    private static long? DictionaryMemoGroupExtension((int Index, int GroupIndex) state, DictionaryMemoRun run)
    {
        var (index, groupIndex) = state;
        var nums = run.Nums;
        var target = run.AndValues[groupIndex];
        long runningAnd = nums[index];
        long? best = null;

        // The AND only ever clears bits, so once it has dropped below the target it can
        // never climb back - the group stops extending there, in the loop bound itself.
        for (var end = index; end < nums.Length && runningAnd >= target; end++)
        {
            runningAnd = FoldNext(nums, runningAnd, end, index);

            if (runningAnd != target)
            {
                continue;
            }

            var tail = DictionaryMemoSum((end + 1, groupIndex + 1), run);
            best = CheaperCompletion(best, nums[end], tail);
        }

        return best;
    }

    // Closing the group at a value whose remainder has no valid split leaves the best so
    // far alone; otherwise the closing value plus that remainder's cost competes with it.
    private static long? CheaperCompletion(long? best, long closingValue, long? tail)
    {
        if (tail is null)
        {
            return best;
        }

        return Cheaper(best, closingValue + tail.Value);
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

    // What the hand-rolled memo run carries through every call: the fixed inputs and
    // the table its lookup and store read and write.
    private readonly record struct DictionaryMemoRun(
        int[] Nums, int[] AndValues, Dictionary<(int Index, int GroupIndex), long?> Memo);

    // The next group's start index and which andValue it must match are the whole state;
    // the array and the targets stay fixed for the call and live here.
    private sealed class MinimumSumFromGroupStart(int[] nums, int[] andValues)
        : IRecurrence<(int Index, int GroupIndex), long?>
    {
        public long? Replay((int Index, int GroupIndex) state, IRecurrence<(int Index, int GroupIndex), long?> rest)
            => PartitionStep(state, nums, andValues, rest);
    }

}
