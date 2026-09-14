using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.LeetCode.ContinuousSubarrays;

// LeetCode 2762. Continuous Subarrays: count the contiguous subarrays whose largest
// and smallest values differ by at most 2.
//
// This is LC 1438's window with the limit pinned at 2 and the answer changed from
// "how long is the best window" to "how many subarrays are there", so both
// strategies answer it the same way that problem's do and differ only in how a
// window learns its max and min. Every window ending at right that satisfies the
// limit contributes (right - left + 1) subarrays, one per admissible starting point,
// so summing that across every right is the whole answer.
internal static class ContinuousSubarraysSolution
{
    // The problem fixes the admissible spread; unlike LC 1438 it is not an argument.
    private const int Limit = 2;

    // The textbook answer: every starting index grows its own window from scratch,
    // tracking a running max/min and counting each endpoint until the spread exceeds
    // the limit. Plain BCL indexing and Math.Max/Min, nothing from this repo - it is
    // the O(n^2) arm the deque strategy below has to justify itself against, and
    // putting it here is what finally gets it asserted.
    public static long CountContinuousSubarraysByBruteForceWindows(int[] nums)
    {
        var total = 0L;

        for (var start = 0; start < nums.Length; start++)
        {
            var windowMax = int.MinValue;
            var windowMin = int.MaxValue;

            for (var end = start; end < nums.Length; end++)
            {
                windowMax = Math.Max(windowMax, nums[end]);
                windowMin = Math.Min(windowMin, nums[end]);

                if (windowMax - windowMin > Limit)
                {
                    break;
                }

                total++;
            }
        }

        return total;
    }

    // Two of this repo's own Deque<int> instances, each a monotonic index window over
    // the same expanding/shrinking range - one decreasing-value (so its front is the
    // window's max) and one increasing-value (so its front is the window's min) - the
    // exact LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit precedent,
    // accumulating window lengths instead of taking the largest. Each index is pushed
    // and popped from each deque at most once, so the whole sweep is O(n) rather than
    // the baseline's O(n^2) rescan.
    public static long CountContinuousSubarraysByMonotonicDeques(int[] nums)
    {
        var window = new MinMaxWindow(nums);
        var total = 0L;

        for (var right = 0; right < nums.Length; right++)
        {
            total += window.Advance(right);
        }

        return total;
    }

    private sealed class MinMaxWindow(int[] nums)
    {
        private readonly RepoDeque _maxWindow = new();
        private readonly RepoDeque _minWindow = new();
        private readonly int[] _nums = nums;
        private int _left;

        // Admits index right, restores the limit, and reports how many subarrays end
        // at right - one per starting point from the left edge through right.
        public int Advance(int right)
        {
            PushMax(right);
            PushMin(right);
            ShrinkToLimit();

            return right - _left + 1;
        }

        private void PushMax(int right)
        {
            while (_maxWindow.TryPeekBack(out var maxBack) && _nums[maxBack] <= _nums[right])
            {
                _maxWindow.TryPopBack(out _);
            }

            _maxWindow.PushBack(right);
        }

        private void PushMin(int right)
        {
            while (_minWindow.TryPeekBack(out var minBack) && _nums[minBack] >= _nums[right])
            {
                _minWindow.TryPopBack(out _);
            }

            _minWindow.PushBack(right);
        }

        private void ShrinkToLimit()
        {
            while (IsSpreadBeyondLimit())
            {
                _left++;
                DropStaleFronts();
            }
        }

        // The live window's spread is the value at the max deque's front minus the
        // value at the min deque's front; an empty deque means there is no window
        // left to measure, so nothing can exceed the limit.
        private bool IsSpreadBeyondLimit()
            => _maxWindow.TryPeekFront(out var maxFront)
                && _minWindow.TryPeekFront(out var minFront)
                && _nums[maxFront] - _nums[minFront] > Limit;

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
