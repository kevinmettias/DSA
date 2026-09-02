using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.CountGoodSubarrays;

// LeetCode 3878. Count Good Subarrays: a subarray is good when the bitwise OR
// of all its elements equals one of its own elements.
//
// OR(l, r) is always >= max(nums[l..r]) as a bit-superset (OR-ing more values
// only adds bits), so OR(l, r) can equal an element only by equalling the
// maximum element - i.e. a subarray is good iff OR(l, r) == max(nums[l..r]).
internal static class CountGoodSubarraysSolution
{
    // The textbook O(n^2) scan: for every start, extend right while tracking
    // the running OR and running max directly, counting whenever they agree.
    // Correct, and the arm the running-OR-groups strategy below has to beat.
    public static long CountGoodSubarraysByBruteForce(int[] nums)
    {
        var count = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            var runningOr = 0;
            var runningMax = 0;

            for (var right = left; right < nums.Length; right++)
            {
                runningOr |= nums[right];
                runningMax = Math.Max(runningMax, nums[right]);

                if (runningOr == runningMax)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // For a fixed right end, OR(l, r) is non-increasing as l grows (a bit
    // superset chain), so the distinct values of OR(l, r) over l in [0, r] form
    // at most ~log2(max value) contiguous l-ranges. This repo's own
    // DynamicArray<T> holds that small run-length-encoded list (TwoSum-style
    // preference for the repo's own containers over a BCL List), rebuilt one
    // right-end at a time by OR-ing every previous run with nums[right] and
    // appending a fresh singleton run for l = right, merging where two
    // neighboring runs land on the same value.
    //
    // A run [lo, hi] with value v is good for every l in it with l <= the last
    // index at or before r where v actually occurs as an element (tracked by
    // this repo's own HashMap<TKey, TValue>, TwoSum precedent) - past that
    // index no element in [l, r] still equals v. That is a single O(1) lookup
    // per run, so the whole pass costs O(n log(max value)).
    public static long CountGoodSubarraysByRunningOrGroups(int[] nums)
    {
        var lastSeen = new HashMap<int, int>();
        var runs = new DynamicArray<OrRun>();
        var count = 0L;

        for (var right = 0; right < nums.Length; right++)
        {
            lastSeen.Set(nums[right], right);
            runs = AdvanceRuns(runs, nums[right], right);
            count += CountGoodStarts(runs, lastSeen);
        }

        return count;
    }

    private static DynamicArray<OrRun> AdvanceRuns(DynamicArray<OrRun> previous, int value, int right)
    {
        var next = new DynamicArray<OrRun>();

        for (var i = 0; i < previous.Count; i++)
        {
            var run = previous.Get(i);
            AppendOrMergeRun(next, run.OrValue | value, run.Lo, run.Hi);
        }

        AppendOrMergeRun(next, value, right, right);
        return next;
    }

    private static void AppendOrMergeRun(DynamicArray<OrRun> runs, int orValue, int lo, int hi)
    {
        if (runs.Count > 0)
        {
            var last = runs.Get(runs.Count - 1);

            if (last.OrValue == orValue)
            {
                runs.Set(runs.Count - 1, new OrRun(orValue, last.Lo, hi));
                return;
            }
        }

        runs.Add(new OrRun(orValue, lo, hi));
    }

    private static long CountGoodStarts(DynamicArray<OrRun> runs, HashMap<int, int> lastSeen)
    {
        var count = 0L;

        for (var i = 0; i < runs.Count; i++)
        {
            var run = runs.Get(i);

            if (!lastSeen.TryGetValue(run.OrValue, out var lastIndex) || lastIndex < run.Lo)
            {
                continue;
            }

            count += Math.Min(run.Hi, lastIndex) - run.Lo + 1;
        }

        return count;
    }

    // One contiguous run of start indices l in [Lo, Hi] that all produce the
    // same OR(l, r) == OrValue for the right end currently being processed.
    // Meaningful only to this problem's running-OR compression.
    private readonly record struct OrRun(int OrValue, int Lo, int Hi);
}
