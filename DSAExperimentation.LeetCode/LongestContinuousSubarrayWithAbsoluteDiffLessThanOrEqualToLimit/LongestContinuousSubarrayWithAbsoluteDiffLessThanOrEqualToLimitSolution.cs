using DSAExperimentation.DataStructures.MonotonicDeque;

namespace DSAExperimentation.LeetCode.LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit;

// LeetCode 1438. Longest Continuous Subarray With Absolute Diff Less Than or Equal
// to Limit: the longest contiguous run whose largest and smallest values differ by
// at most limit.
//
// Both strategies answer the same question and differ only in how they learn a
// window's max and min: the baseline rescans each window from its own start, while
// the composed strategy keeps them incrementally in two of this repo's MonotonicDeques.
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

    // Two MonotonicDeque instances over the same expanding/shrinking range - one under
    // MaxWindowOrder (so its front is the window's max) and one under MinWindowOrder
    // (so its front is the window's min) - the exact SlidingWindowMaximum precedent
    // doubled up. The spread is always the max front's value minus the min front's;
    // once it exceeds limit the left edge advances
    // until it doesn't, with each index pushed and popped from each deque at most
    // once for O(n) total instead of the baseline's O(n^2) rescan.
    public static int LongestSubarrayByMonotonicDeques(int[] nums, int limit)
    {
        var window = new MinMaxWindow(nums, limit);
        var best = 0;

        for (var right = 0; right < nums.Length; right++)
        {
            best = Math.Max(best, window.Advance(right));
        }

        return best;
    }

    private sealed class MinMaxWindow(int[] nums, int limit)
    {
        private readonly MonotonicDeque<int, MaxWindowOrder<int>> _maxWindow = new();
        private readonly MonotonicDeque<int, MinWindowOrder<int>> _minWindow = new();
        private int _left;

        // Admits index right, restores the limit, and reports the resulting window's
        // length.
        public int Advance(int right)
        {
            _maxWindow.Push(right, nums[right]);
            _minWindow.Push(right, nums[right]);
            ShrinkToLimit();

            return right - _left + 1;
        }

        // Each step past the left edge drops whatever front entries it leaves behind
        // in both deques.
        private void ShrinkToLimit()
        {
            while (IsWindowSpreadOverLimit(_maxWindow, _minWindow, limit))
            {
                _left++;
                _maxWindow.EvictBefore(_left);
                _minWindow.EvictBefore(_left);
            }
        }

        // The window is over its limit while both deques still hold a front and the spread
        // between those two fronts - largest value minus smallest - is past limit.
        private static bool IsWindowSpreadOverLimit(
            MonotonicDeque<int, MaxWindowOrder<int>> maxWindow,
            MonotonicDeque<int, MinWindowOrder<int>> minWindow,
            int limit) =>
            maxWindow.TryPeekFront(out var maximum) && minWindow.TryPeekFront(out var minimum) &&
            maximum.Key - minimum.Key > limit;
    }
}
