using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ContinuousSubarrays;

// LeetCode 2762. Continuous Subarrays: a subarray is continuous when its max and min
// differ by at most 2. Two of this repo's own Deque<int> instances, each a monotonic
// index window over the same expanding/shrinking range - one decreasing-value (running
// max) and one increasing-value (running min) - the exact
// LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitTests precedent, with
// Limit fixed at 2. Every window ending at right that satisfies the limit contributes
// (right - left + 1) subarrays (one per possible starting point from left through
// right), so summing that across every right instead of tracking the single longest one
// answers this problem with the identical sliding mechanism, each index still
// pushed/popped from each deque at most once for O(n) total.
public sealed class ContinuousSubarraysTests
{
    private const int Limit = 2;

    [Fact]
    public void CountContinuousSubarrays_LeetCodeExampleOne_ReturnsEight()
    {
        int[] nums = [5, 4, 2, 4];

        var result = CountContinuousSubarrays(nums);

        Assert.Equal(8, result);
    }

    [Fact]
    public void CountContinuousSubarrays_LeetCodeExampleTwo_EveryPairWithinLimit_ReturnsAllSubarrays()
    {
        int[] nums = [1, 2, 3];

        var result = CountContinuousSubarrays(nums);

        Assert.Equal(6, result);
    }

    [Fact]
    public void CountContinuousSubarrays_AllValuesEqual_EveryPossibleSubarrayCounts()
    {
        int[] nums = [1, 1, 1, 1];

        var result = CountContinuousSubarrays(nums);

        Assert.Equal(10, result);
    }

    [Fact]
    public void CountContinuousSubarrays_SingleElement_ReturnsOne()
    {
        int[] nums = [7];

        var result = CountContinuousSubarrays(nums);

        Assert.Equal(1, result);
    }

    private static long CountContinuousSubarrays(int[] nums)
    {
        var window = new MinMaxWindow();
        var total = 0L;

        for (var right = 0; right < nums.Length; right++)
        {
            total += window.Advance(right, nums);
        }

        return total;
    }

    private sealed class MinMaxWindow
    {
        private readonly RepoDeque _maxWindow = new();
        private readonly RepoDeque _minWindow = new();
        private int _left;

        public int Advance(int right, int[] nums)
        {
            PushMax(right, nums);
            PushMin(right, nums);
            ShrinkToLimit(nums);

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

        private void ShrinkToLimit(int[] nums)
        {
            while (_maxWindow.TryPeekFront(out var maxFront) && _minWindow.TryPeekFront(out var minFront)
                && nums[maxFront] - nums[minFront] > Limit)
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
