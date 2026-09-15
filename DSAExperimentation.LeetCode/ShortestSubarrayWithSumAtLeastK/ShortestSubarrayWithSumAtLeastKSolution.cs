using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.ShortestSubarrayWithSumAtLeastK;

// LeetCode 862. Shortest Subarray with Sum at Least K: the length of the shortest
// non-empty contiguous subarray whose sum is at least k, or -1 when none exists.
// Values may be negative, so a plain sliding window does not apply - the window sum
// is no longer monotone as the window grows.
//
// Both strategies start from the same prefix-sum array: a subarray (i, j] sums to
// prefix[j] - prefix[i], so the question becomes "for each j, the largest i < j with
// prefix[j] - prefix[i] >= k". The baseline answers that by scanning every pair,
// O(n^2). The composed strategy keeps candidate prefix indices in this repo's own
// Deque<int> as a monotonic increasing-prefix-value window - the same "Deque<int> as
// a monotonic index window" composition SlidingWindowMaximumSolution establishes for
// LC 239, applied to prefix-sum indices instead of raw values. Each index is pushed
// and popped at most once, giving O(n).
internal static class ShortestSubarrayWithSumAtLeastKSolution
{
    // The textbook answer: build the prefix sums, then for every start scan forward
    // for the first end that reaches k. Deliberately written with nothing but BCL
    // arrays - it is the arm the composed strategy below has to justify itself against.
    public static int ShortestSubarrayByBruteForcePrefixScan(int[] nums, int k)
    {
        var prefix = BuildPrefixSums(nums);
        var best = NoWindow(nums.Length);

        for (var start = 0; start < prefix.Length; start++)
        {
            var candidate = ShortestWindowFrom(prefix, start, k);

            best = Math.Min(best, candidate);
        }

        return ReportLength(best, nums.Length);
    }

    // The first end that reaches k is also the shortest one from this start, so the
    // inner scan stops there rather than running to the end of the array.
    private static int ShortestWindowFrom(long[] prefix, int start, int k)
    {
        for (var end = start + 1; end < prefix.Length; end++)
        {
            if (prefix[end] - prefix[start] >= k)
            {
                return end - start;
            }
        }

        return NoWindow(prefix.Length - 1);
    }

    // This repo's own Deque<int> holding prefix indices in increasing-prefix-value
    // order. The front is popped while it already answers the current index (no later
    // index can pair with it more cheaply), and the back is popped while it holds a
    // prefix no smaller than the current one (a larger, earlier prefix can never beat
    // the current index as a window start).
    public static int ShortestSubarrayByMonotonicDeque(int[] nums, int k)
    {
        var prefix = BuildPrefixSums(nums);
        var best = NoWindow(nums.Length);
        var window = new RepoDeque();

        for (var i = 0; i < prefix.Length; i++)
        {
            var candidate = ClaimReachableStarts(prefix, window, i, k);

            best = Math.Min(best, candidate);
            DropDominatedStarts(prefix, window, i);
            window.PushBack(i);
        }

        return ReportLength(best, nums.Length);
    }

    private static int ClaimReachableStarts(long[] prefix, RepoDeque window, int i, int k)
    {
        var best = NoWindow(prefix.Length - 1);

        while (window.TryPeekFront(out var frontIndex) && prefix[i] - prefix[frontIndex] >= k)
        {
            best = Math.Min(best, i - frontIndex);
            window.TryPopFront(out _);
        }

        return best;
    }

    private static void DropDominatedStarts(long[] prefix, RepoDeque window, int i)
    {
        while (window.TryPeekBack(out var backIndex) && prefix[backIndex] >= prefix[i])
        {
            window.TryPopBack(out _);
        }
    }

    private static long[] BuildPrefixSums(int[] nums)
    {
        var prefix = new long[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + nums[i];
        }

        return prefix;
    }

    // One past the longest possible subarray, so it loses every Math.Min against a
    // real window and still reads as "nothing found" at the end.
    private static int NoWindow(int length) => length + 1;

    private static int ReportLength(int best, int length) => best > length ? LeetCodeAnswer.None : best;
}
