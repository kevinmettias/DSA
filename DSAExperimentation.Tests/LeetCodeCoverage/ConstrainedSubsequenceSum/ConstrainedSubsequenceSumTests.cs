using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstrainedSubsequenceSum;

// LeetCode 1425. Constrained Subsequence Sum: dp[i] = nums[i] + max(0, the best
// dp value in the trailing window [i-k, i-1]). That window maximum is maintained
// exactly the way SlidingWindowMaximumTests.cs already does it - this repo's own
// Deque<int> holding indices in decreasing dp-value order (a monotonic deque) -
// except the values being compared (dp[i]) are computed on the fly as part of
// the same loop instead of being read from a fixed input array up front, so
// push/pop interleaves with the dp recurrence itself. Every index is still
// pushed and popped at most once, giving O(n) total instead of the O(n*k)
// per-position rescan.
public sealed class ConstrainedSubsequenceSumTests
{
    [Fact]
    public void MaxSum_ClassicExample_ReturnsMaximumConstrainedSubsequenceSum()
    {
        int[] nums = [10, 2, -10, 5, 20];

        var result = ConstrainedSubsetSum(nums, k: 2);

        Assert.Equal(37, result);
    }

    [Fact]
    public void MaxSum_AllNegative_MustStillPickOneElement_ReturnsLargestSingleValue()
    {
        int[] nums = [-1, -2, -3];

        var result = ConstrainedSubsetSum(nums, k: 1);

        Assert.Equal(-1, result);
    }

    private static int ConstrainedSubsetSum(int[] nums, int k)
    {
        var dp = new int[nums.Length];
        var window = new RepoDeque();
        var best = int.MinValue;

        for (var i = 0; i < nums.Length; i++)
        {
            if (window.TryPeekFront(out var frontIndex) && frontIndex < i - k)
            {
                window.TryPopFront(out _);
            }

            var windowMax = 0;
            if (window.TryPeekFront(out var maxIndex))
            {
                windowMax = Math.Max(0, dp[maxIndex]);
            }

            dp[i] = nums[i] + windowMax;
            best = Math.Max(best, dp[i]);

            while (window.TryPeekBack(out var backIndex) && dp[backIndex] <= dp[i])
            {
                window.TryPopBack(out _);
            }

            window.PushBack(i);
        }

        return best;
    }
}
