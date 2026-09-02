using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameVI;

// LeetCode 1696. Jump Game VI: dp[i] = nums[i] + the best dp value reachable
// from a jump of at most k steps, i.e. the max over the trailing window
// [i-k, i-1]. That window maximum is maintained the same way
// ConstrainedSubsequenceSumTests.cs already does it for LC 1425 - this repo's
// own Deque<int> holding indices in decreasing dp-value order (a monotonic
// deque), with push/pop interleaved into the same loop as the dp recurrence
// itself, since the compared values (dp[i]) are computed on the fly. Every
// index is pushed and popped at most once, giving O(n) total instead of the
// O(n*k) per-position rescan. Unlike LC 1425, every step here is a mandatory
// jump (never "skip this index"), so the window max has no floor-at-zero.
public sealed class JumpGameVITests
{
    [Fact]
    public void MaxResult_LeetCodeExampleOne_ReturnsSeven()
    {
        int[] nums = [1, -1, -2, 4, -7, 3];

        var result = MaxResult(nums, k: 2);

        Assert.Equal(7, result);
    }

    [Fact]
    public void MaxResult_LeetCodeExampleTwo_ReturnsSeventeen()
    {
        int[] nums = [10, -5, -2, 4, 0, 3];

        var result = MaxResult(nums, k: 3);

        Assert.Equal(17, result);
    }

    [Fact]
    public void MaxResult_LeetCodeExampleThree_ReturnsZero()
    {
        int[] nums = [1, -5, -20, 4, -1, 3, -6, -3];

        var result = MaxResult(nums, k: 2);

        Assert.Equal(0, result);
    }

    private static int MaxResult(int[] nums, int k)
    {
        var dp = new int[nums.Length];
        dp[0] = nums[0];
        var window = new MaxResultWindow(dp, k);
        window.Seed(0);

        for (var i = 1; i < nums.Length; i++)
        {
            window.Advance(i, nums[i]);
        }

        return dp[^1];
    }

    private sealed class MaxResultWindow(int[] dp, int k)
    {
        private readonly RepoDeque _window = new();

        public void Seed(int index) => _window.PushBack(index);

        public void Advance(int i, int value)
        {
            while (_window.TryPeekFront(out var frontIndex) && frontIndex < i - k)
            {
                _window.TryPopFront(out _);
            }

            _window.TryPeekFront(out var maxIndex);
            dp[i] = value + dp[maxIndex];

            while (_window.TryPeekBack(out var backIndex) && dp[backIndex] <= dp[i])
            {
                _window.TryPopBack(out _);
            }

            _window.PushBack(i);
        }
    }
}
