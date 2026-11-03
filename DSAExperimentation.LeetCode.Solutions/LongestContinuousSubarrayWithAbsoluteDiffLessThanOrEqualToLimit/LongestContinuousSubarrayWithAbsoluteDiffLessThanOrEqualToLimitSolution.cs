using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.LeetCode.Solutions.LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit;

// LeetCode 1438. Longest Continuous Subarray With Absolute Diff Less Than or Equal
// to Limit: the longest contiguous run whose largest and smallest values differ by
// at most limit.
//
// Both strategies answer the same question and differ only in how they learn a
// window's max and min: the baseline rescans each window from its own start, while
// the composed strategy keeps them incrementally in this repo's MinMaxWindow.
[LeetCodeQuestion("longest-continuous-subarray-with-absolute-diff-less-than-or-equal-to-limit", LeetCodeAnswerOrder.AsWritten)]
internal static class LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution
{
    // The textbook answer: every starting index grows its own window from scratch,
    // tracking a running max/min until the spread exceeds limit. Plain BCL indexing
    // and Math.Max/Min, nothing from this repo - it is the arm the deque strategy
    // below has to justify itself against, and putting it here is what finally gets
    // it asserted.
    public static int LongestSubarrayByBruteForceWindows(int[] nums, int limit)
    {
        var best = 0;

        for (var start = 0; start < nums.Length; start++)
        {
            var windowMax = int.MinValue;
            var windowMin = int.MaxValue;

            for (var end = start; end < nums.Length; end++)
            {
                windowMax = Math.Max(windowMax, nums[end]);
                windowMin = Math.Min(windowMin, nums[end]);

                if (windowMax - windowMin > limit)
                {
                    break;
                }

                best = Math.Max(best, end - start + 1);
            }
        }

        return best;
    }

    // A MinMaxWindow over the expanding/shrinking range - two MonotonicDeques inside,
    // one under each window order, the SlidingWindowMaximum precedent doubled up. The
    // spread is the window's max minus its min; once it exceeds limit the left edge
    // advances until it doesn't, with each index pushed and popped from each deque at
    // most once for O(n) total instead of the baseline's O(n^2) rescan.
    public static int LongestSubarrayByMonotonicDeques(int[] nums, int limit)
    {
        var window = new MinMaxWindow<int>();
        var left = 0;
        var best = 0;

        for (var right = 0; right < nums.Length; right++)
        {
            window.Push(right, nums[right]);
            left = ShrinkToLimit(window, limit, left);
            best = Math.Max(best, right - left + 1);
        }

        return best;
    }

    // Each step past the left edge drops whatever entries it leaves behind; the
    // advanced left edge comes back as the result.
    private static int ShrinkToLimit(MinMaxWindow<int> window, int limit, int left)
    {
        while (IsWindowSpreadOverLimit(window, limit))
        {
            left++;
            window.EvictBefore(left);
        }

        return left;
    }

    // The window is over its limit while it still holds a value and its spread -
    // largest value minus smallest - is past limit.
    private static bool IsWindowSpreadOverLimit(MinMaxWindow<int> window, int limit) =>
        window.TryPeekRange(out var minimum, out var maximum) && maximum - minimum > limit;
}
