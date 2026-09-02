using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestSubarrayWithSumAtLeastK;

// LeetCode 862. Shortest Subarray with Sum at Least K: prefix sums plus this
// repo's own Deque<int>, holding prefix-sum indices in increasing-prefix-value
// order (a monotonic deque) - the same "Deque<int> as a monotonic index window"
// composition SlidingWindowMaximumTests.cs already establishes for LC239, applied
// here to prefix-sum indices instead of raw values so it also handles negative
// array elements (a plain sliding window alone can't, since the window sum is no
// longer monotone as it grows).
public sealed class ShortestSubarrayWithSumAtLeastKTests
{
    [Theory]
    [InlineData(new[] { 1 }, 1, 1)]
    [InlineData(new[] { 1, 2 }, 4, -1)]
    [InlineData(new[] { 2, -1, 2 }, 3, 3)]
    public void ShortestSubarray_LeetCodeExamples_ReturnsExpectedLength(int[] nums, int k, int expected)
    {
        var actual = ShortestSubarray(nums, k);
        Assert.Equal(expected, actual);
    }

    private static int ShortestSubarray(int[] nums, int k)
    {
        var prefix = BuildPrefixSums(nums);
        var best = FindShortestWindow(prefix, k);

        return best > nums.Length ? -1 : best;
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

    private static int FindShortestWindow(long[] prefix, int k)
    {
        var n = prefix.Length - 1;
        var best = n + 1;
        var window = new RepoDeque();

        for (var i = 0; i <= n; i++)
        {
            while (window.TryPeekFront(out var frontIndex) && prefix[i] - prefix[frontIndex] >= k)
            {
                best = Math.Min(best, i - frontIndex);
                window.TryPopFront(out _);
            }

            while (window.TryPeekBack(out var backIndex) && prefix[backIndex] >= prefix[i])
            {
                window.TryPopBack(out _);
            }

            window.PushBack(i);
        }

        return best;
    }
}
