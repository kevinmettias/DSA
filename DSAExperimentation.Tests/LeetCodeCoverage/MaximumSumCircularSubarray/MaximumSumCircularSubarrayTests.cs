namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumCircularSubarray;

// LeetCode 918. Maximum Sum Circular Subarray: MaximumSubarray's own single Kadane
// pass (LC 53's precedent), run twice over the same array - once for the best
// NON-wraparound subarray sum (unchanged), once for the WORST subarray sum (the
// same recurrence with Math.Min instead of Math.Max) - so the best WRAPAROUND
// subarray reduces to total - worstSum, the standard complement trick (a
// wraparound subarray is exactly "everything except some non-wraparound middle
// stretch"). All-negative input is the one case where that complement is wrong (it
// would credit an empty wraparound subarray, which isn't a legal answer), caught by
// falling back to the plain maxSum whenever it's still negative.
public sealed partial class MaximumSumCircularSubarrayTests
{
    [Theory]
    [InlineData(new[] { 1, -2, 3, -2 }, 3)]
    [InlineData(new[] { 5, -3, 5 }, 10)]
    [InlineData(new[] { -3, -2, -3 }, -2)]
    public void MaxSubarraySumCircular_LeetCodeExamples_ReturnsBestCircularSum(int[] nums, int expected)
        => Assert.Equal(expected, MaxSubarraySumCircular(nums));

    private static int MaxSubarraySumCircular(int[] nums)
    {
        var total = 0;
        var maxSum = nums[0];
        var currentMax = 0;
        var minSum = nums[0];
        var currentMin = 0;

        foreach (var n in nums)
        {
            currentMax = Math.Max(n, currentMax + n);
            maxSum = Math.Max(maxSum, currentMax);

            currentMin = Math.Min(n, currentMin + n);
            minSum = Math.Min(minSum, currentMin);

            total += n;
        }

        return maxSum < 0 ? maxSum : Math.Max(maxSum, total - minSum);
    }
}
