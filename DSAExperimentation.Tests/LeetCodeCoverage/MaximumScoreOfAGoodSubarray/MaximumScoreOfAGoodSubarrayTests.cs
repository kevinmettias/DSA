using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumScoreOfAGoodSubarray;

// LeetCode 1793. Maximum Score of a Good Subarray: two monotonic-increasing
// Stack<int> sweeps (DailyTemperatures/CarFleetII precedent for this repo's own
// Stack) compute, for every index, the nearest strictly-smaller element to its left
// and right - the widest window where nums[i] is the minimum. The answer is the
// best nums[i] * width among the indices whose window actually contains k, since
// the optimal good subarray's minimum must be one of those windows' defining index.
public sealed partial class MaximumScoreOfAGoodSubarrayTests
{
    [Fact]
    public void MaximumScore_ClassicExample_ReturnsFifteen()
    {
        int[] nums = [1, 4, 3, 7, 4, 5];

        var score = MaximumScore(nums, k: 3);

        Assert.Equal(15, score);
    }

    [Fact]
    public void MaximumScore_PlateauWithDuplicates_ReturnsTwenty()
    {
        int[] nums = [5, 5, 4, 5, 4, 1, 1, 1];

        var score = MaximumScore(nums, k: 0);

        Assert.Equal(20, score);
    }

    private static int MaximumScore(int[] nums, int k)
    {
        var previousSmaller = BoundaryIndices(nums, left: true);
        var nextSmaller = BoundaryIndices(nums, left: false);

        var best = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if (previousSmaller[i] < k && k < nextSmaller[i])
            {
                var width = nextSmaller[i] - previousSmaller[i] - 1;
                best = Math.Max(best, nums[i] * width);
            }
        }

        return best;
    }

    // left: true computes, per index, the nearest strictly-smaller index to its
    // left (or -1); false computes the nearest strictly-smaller index to its right
    // (or nums.Length) - the same monotonic-stack sweep run in each direction.
    private static int[] BoundaryIndices(int[] nums, bool left)
    {
        var n = nums.Length;
        var result = new int[n];
        var stack = new RepoIntStack();
        var outOfRange = left ? -1 : n;

        for (var step = 0; step < n; step++)
        {
            var i = left ? step : n - 1 - step;

            while (stack.TryPeek(out var top) && nums[top] >= nums[i])
            {
                stack.TryPop(out _);
            }

            result[i] = stack.TryPeek(out var boundary) ? boundary : outOfRange;
            stack.Push(i);
        }

        return result;
    }
}
