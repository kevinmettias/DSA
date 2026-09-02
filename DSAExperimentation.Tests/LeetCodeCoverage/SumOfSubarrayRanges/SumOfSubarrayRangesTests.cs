using RangeIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfSubarrayRanges;

// LeetCode 2104. Sum of Subarray Ranges: sum(subarray max) - sum(subarray min),
// each computed in O(n) via the same monotonic-stack "distance to the next/previous
// out-of-order element" contribution trick LargestRectangleInHistogramTests already
// uses, over this repo's own Stack<int> holding indices (not values).
public sealed partial class SumOfSubarrayRangesTests
{
    [Fact]
    public void SubarrayRanges_ClassicExample_ReturnsSumOfRanges()
    {
        int[] nums = [1, 2, 3];

        Assert.Equal(4, SubarrayRanges(nums));
    }

    [Fact]
    public void SubarrayRanges_WithDuplicatesAndNegatives_ReturnsSumOfRanges()
    {
        int[] nums = [4, -2, -3, 4, 1];

        Assert.Equal(59, SubarrayRanges(nums));
    }

    private static long SubarrayRanges(int[] nums) => SumOfSubarrayMaximums(nums) - SumOfSubarrayMinimums(nums);

    // Pop condition is strict (<) so a run of equal maximums only ever gets
    // attributed once, to the leftmost occurrence - the same duplicate-safe shape
    // SumOfSubarrayMinimums mirrors with the opposite strict comparison.
    private static long SumOfSubarrayMaximums(int[] nums) =>
        SumOfSubarrayContribution(nums, int.MaxValue, static (value, current) => value < current);

    private static long SumOfSubarrayMinimums(int[] nums) =>
        SumOfSubarrayContribution(nums, int.MinValue, static (value, current) => value > current);

    // Shared monotonic-stack shape behind SumOfSubarrayMaximums/Minimums: only the
    // "no more elements" sentinel and the pop condition differ between max and min.
    private static long SumOfSubarrayContribution(int[] nums, int sentinel, Func<int, int, bool> shouldPop)
    {
        var sum = 0L;
        var indices = new RangeIndexStack();

        for (var i = 0; i <= nums.Length; i++)
        {
            var current = i == nums.Length ? sentinel : nums[i];

            while (indices.TryPeek(out var top) && shouldPop(nums[top], current))
            {
                indices.TryPop(out _);
                var left = indices.TryPeek(out var previous) ? previous : -1;
                sum += (long)nums[top] * (top - left) * (i - top);
            }

            indices.Push(i);
        }

        return sum;
    }
}
