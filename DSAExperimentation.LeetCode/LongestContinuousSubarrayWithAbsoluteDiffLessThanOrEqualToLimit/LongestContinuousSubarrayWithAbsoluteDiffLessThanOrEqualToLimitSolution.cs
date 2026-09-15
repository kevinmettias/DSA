using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit;

// LeetCode 1438. Longest Continuous Subarray With Absolute Diff Less Than or Equal
// to Limit: the longest contiguous run whose largest and smallest values differ by
// at most limit.
//
// Both strategies answer the same question and differ only in how they learn a
// window's max and min: the baseline rescans each window from its own start, while
// the composed strategy keeps them incrementally in two of this repo's own Deques.
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

    // Two Deque<int> instances, each a monotonic index window over the same
    // expanding/shrinking range - one decreasing-value (so its front is the window's
    // max) and one increasing-value (so its front is the window's min) - the exact
    // SlidingWindowMaximum precedent doubled up. The spread is always
    // nums[maxFront] - nums[minFront]; once it exceeds limit the left edge advances
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
        private readonly RepoDeque _maxWindow = new();
        private readonly RepoDeque _minWindow = new();
        private int _left;

        // Admits index right, restores the limit, and reports the resulting window's
        // length.
        public int Advance(int right)
        {
            PushMax(right);
            PushMin(right);
            ShrinkToLimit();

            return right - _left + 1;
        }

        private void PushMax(int right)
        {
            while (_maxWindow.TryPeekBack(out var maxBack) && nums[maxBack] <= nums[right])
            {
                _maxWindow.TryPopBack(out _);
            }

            _maxWindow.PushBack(right);
        }

        private void PushMin(int right)
        {
            while (_minWindow.TryPeekBack(out var minBack) && nums[minBack] >= nums[right])
            {
                _minWindow.TryPopBack(out _);
            }

            _minWindow.PushBack(right);
        }

        private void ShrinkToLimit()
        {
            while (WindowSpreadExceedsLimit(_maxWindow, _minWindow, nums, limit))
            {
                _left++;
                DropStaleFronts();
            }
        }

        // The window is over its limit while both deques still hold a front and the spread
        // between those two fronts - largest value minus smallest - is past limit.
        private static bool WindowSpreadExceedsLimit(
            RepoDeque maxWindow, RepoDeque minWindow, int[] nums, int limit) =>
            maxWindow.TryPeekFront(out var maxFront) && minWindow.TryPeekFront(out var minFront) &&
            nums[maxFront] - nums[minFront] > limit;

        // At most one index per deque can fall behind the left edge per step, since
        // the edge advances by one.
        private void DropStaleFronts()
        {
            if (_maxWindow.TryPeekFront(out var maxFront) && maxFront < _left)
            {
                _maxWindow.TryPopFront(out _);
            }

            if (_minWindow.TryPeekFront(out var minFront) && minFront < _left)
            {
                _minWindow.TryPopFront(out _);
            }
        }
    }
}
