using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSubarrayMinProduct;

// LeetCode 1856. Maximum Subarray Min-Product: the same monotonic-stack
// contribution technique as Sum of Subarray Minimums (LC 907) - two passes
// over this repo's own Stack<int> of pending indices find each element's
// maximal span as the minimum, then a running prefix sum turns "min * sum
// over that span" into O(1) per element instead of re-summing it, O(n)
// overall instead of an O(n^2) every-subarray scan. Only the max is kept
// (not summed), so a symmetric strict-less rule on both sides is safe -
// unlike LC 907's own </<= asymmetry, there is no double-counting risk to
// guard against when comparing rather than accumulating.
public sealed partial class MaximumSubarrayMinProductTests
{
    private const int Modulus = 1_000_000_007;

    [Fact]
    public void MaxSumMinProduct_LeetCodeExampleOne_ReturnsFourteen()
        => Assert.Equal(14, MaxSumMinProduct([1, 2, 3, 2]));

    [Fact]
    public void MaxSumMinProduct_LeetCodeExampleTwo_ReturnsEighteen()
        => Assert.Equal(18, MaxSumMinProduct([2, 3, 3, 1, 2]));

    [Fact]
    public void MaxSumMinProduct_SingleElement_ReturnsItsSquare()
        => Assert.Equal(49, MaxSumMinProduct([7]));

    private static int MaxSumMinProduct(int[] nums)
    {
        var n = nums.Length;
        var prefixSum = new long[n + 1];
        for (var i = 0; i < n; i++)
        {
            prefixSum[i + 1] = prefixSum[i] + nums[i];
        }

        var leftBound = ComputeInclusiveLeftBounds(nums);
        var rightBound = ComputeExclusiveRightBounds(nums);

        var best = 0L;
        for (var i = 0; i < n; i++)
        {
            var sum = prefixSum[rightBound[i]] - prefixSum[leftBound[i]];
            best = Math.Max(best, nums[i] * sum);
        }

        return (int)(best % Modulus);
    }

    private static int[] ComputeInclusiveLeftBounds(int[] nums)
    {
        var bounds = new int[nums.Length];
        var stack = new RepoIntStack();

        for (var i = 0; i < nums.Length; i++)
        {
            while (stack.TryPeek(out var top) && nums[top] >= nums[i])
            {
                stack.TryPop(out _);
            }

            bounds[i] = stack.TryPeek(out var previous) ? previous + 1 : 0;
            stack.Push(i);
        }

        return bounds;
    }

    private static int[] ComputeExclusiveRightBounds(int[] nums)
    {
        var bounds = new int[nums.Length];
        var stack = new RepoIntStack();

        for (var i = nums.Length - 1; i >= 0; i--)
        {
            while (stack.TryPeek(out var top) && nums[top] >= nums[i])
            {
                stack.TryPop(out _);
            }

            bounds[i] = stack.TryPeek(out var next) ? next : nums.Length;
            stack.Push(i);
        }

        return bounds;
    }
}
