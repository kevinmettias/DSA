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
        var memo = new Dictionary<(int Index, int GroupIndex), long?>();

        long? Recurse((int Index, int GroupIndex) state)
        {
            if (memo.TryGetValue(state, out var cached))
            {
                return cached;
            }

            var result = PartitionStep(state, nums, andValues, Recurse);
            memo[state] = result;
            return result;
        }

        return Recurse((0, 0)) ?? LeetCodeAnswer.None;
    }

    // Same recurrence, routed through this repo's own Memoizer so each
    // distinct (Index, GroupIndex) state is solved once without hand-writing
    // the cache lookup/store around every call.
    public static long MinimumValueSumByMemoizedPartition(int[] nums, int[] andValues)
    {
        long? Recurrence((int Index, int GroupIndex) state, Func<(int Index, int GroupIndex), long?> solveRest) =>
            PartitionStep(state, nums, andValues, solveRest);

        var result = Memoizer.Memoize<(int Index, int GroupIndex), long?>((0, 0), Recurrence);

        return result ?? LeetCodeAnswer.None;
    }

    // null means "no valid partition from here" - the same nullable-over-
    // sentinel choice ARCHITECTURE.md records for HammingSearch, so this
    // shared step never has to agree on a magic "impossible" value with
    // either caller.
    private static long? PartitionStep(
        (int Index, int GroupIndex) state,
        int[] nums,
        int[] andValues,
        Func<(int Index, int GroupIndex), long?> solveRest)
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

        long? best = null;
        var runningAnd = nums[index];

        for (var end = index; end < nums.Length; end++)
        {
            if (end > index)
            {
                runningAnd &= nums[end];
            }

            if (runningAnd < andValues[groupIndex])
            {
                break;
            }

            if (runningAnd != andValues[groupIndex])
            {
                continue;
            }

            var tail = solveRest((end + 1, groupIndex + 1));

            if (tail is null)
            {
                continue;
            }

            var candidate = nums[end] + tail.Value;

            if (best is null || candidate < best)
            {
                best = candidate;
            }
        }

        return best;
    }
}
