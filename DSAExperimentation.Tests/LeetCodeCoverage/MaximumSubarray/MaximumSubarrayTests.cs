namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSubarray;

public sealed class MaximumSubarrayTests
{
    [Theory]
    [InlineData(new[] { -2,1,-3,4,-1,2,1,-5,4 }, 6)]
    [InlineData(new[] { 1 }, 1)]
    [InlineData(new[] { 5,4,-1,7,8 }, 23)]
    public void MaxSubArray_KadaneScan_ReturnsBestContiguousSum(int[] nums, int expected) => Assert.Equal(expected, MaxSubArray(nums));

    private static int MaxSubArray(int[] nums)
    {
        var best = nums[0]; var current = nums[0];
        for (var i = 1; i < nums.Length; i++) { current = Math.Max(nums[i], current + nums[i]); best = Math.Max(best, current); }
        return best;
    }
}
