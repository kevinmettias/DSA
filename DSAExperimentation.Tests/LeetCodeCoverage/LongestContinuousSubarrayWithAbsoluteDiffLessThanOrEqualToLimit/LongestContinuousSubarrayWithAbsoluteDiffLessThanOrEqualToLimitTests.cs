using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit;

// LeetCode 1438. Longest Continuous Subarray With Absolute Diff Less Than or Equal to
// Limit: two of this repo's own Deque<int> instances, each a monotonic index window
// over the same expanding/shrinking range - one decreasing-value (running max) and
// one increasing-value (running min), the exact SlidingWindowMaximum precedent
// doubled up. The window's max-min is always front(maxDeque)-front(minDeque); once
// that exceeds limit the left edge advances until it doesn't, with each index still
// pushed/popped from each deque at most once for O(n) total instead of an O(n*k)
// per-window rescan.
public sealed class LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitTests
{
    [Fact]
    public void LongestSubarray_ClassicExampleWithLimitTwo_ReturnsThree()
    {
        int[] nums = [8, 2, 4, 7];

        var result = LongestSubarray(nums, limit: 4);

        Assert.Equal(2, result);
    }

    [Fact]
    public void LongestSubarray_LimitZeroRequiresConstantRun_ReturnsLongestRunOfEqualValues()
    {
        int[] nums = [4, 2, 2, 2, 4, 4, 2, 2];

        var result = LongestSubarray(nums, limit: 0);

        Assert.Equal(3, result);
    }

    [Fact]
    public void LongestSubarray_EntireArrayWithinLimit_ReturnsWholeLength()
    {
        int[] nums = [4, 8, 5, 1, 7, 9];

        var result = LongestSubarray(nums, limit: 9);

        Assert.Equal(6, result);
    }

    private static int LongestSubarray(int[] nums, int limit)
    {
        var window = new MinMaxWindow();
        var best = 0;

        for (var right = 0; right < nums.Length; right++)
        {
            var length = window.Advance(right, nums, limit);
            best = Math.Max(best, length);
        }

        return best;
    }

    private sealed class MinMaxWindow
    {
        private readonly RepoDeque _maxWindow = new();
        private readonly RepoDeque _minWindow = new();
        private int _left;

        public int Advance(int right, int[] nums, int limit)
        {
            PushMax(right, nums);
            PushMin(right, nums);
            ShrinkToLimit(nums, limit);

            return right - _left + 1;
        }

        private void PushMax(int right, int[] nums)
        {
            while (_maxWindow.TryPeekBack(out var maxBack) && nums[maxBack] <= nums[right])
            {
                _maxWindow.TryPopBack(out _);
            }

            _maxWindow.PushBack(right);
        }

        private void PushMin(int right, int[] nums)
        {
            while (_minWindow.TryPeekBack(out var minBack) && nums[minBack] >= nums[right])
            {
                _minWindow.TryPopBack(out _);
            }

            _minWindow.PushBack(right);
        }

        private void ShrinkToLimit(int[] nums, int limit)
        {
            while (_maxWindow.TryPeekFront(out var maxFront) && _minWindow.TryPeekFront(out var minFront)
                && nums[maxFront] - nums[minFront] > limit)
            {
                _left++;

                if (_maxWindow.TryPeekFront(out var frontIndex) && frontIndex < _left)
                {
                    _maxWindow.TryPopFront(out _);
                }

                if (_minWindow.TryPeekFront(out var frontIndex2) && frontIndex2 < _left)
                {
                    _minWindow.TryPopFront(out _);
                }
            }
        }
    }
}
