namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProductSubarray;

public sealed class MaximumProductSubarrayTests
{
    [Theory]
    [InlineData(new[] { 2,3,-2,4 }, 6)]
    [InlineData(new[] { -2,0,-1 }, 0)]
    [InlineData(new[] { -2,3,-4 }, 24)]
    public void MaxProduct_TracksMinAndMaxProducts_ReturnsBestProduct(int[] nums, int expected) => Assert.Equal(expected, MaxProduct(nums));

    private static int MaxProduct(int[] nums)
    {
        var min = nums[0]; var max = nums[0]; var best = nums[0];
        for (var i = 1; i < nums.Length; i++)
        {
            if (nums[i] < 0) (min, max) = (max, min);
            max = Math.Max(nums[i], max * nums[i]); min = Math.Min(nums[i], min * nums[i]); best = Math.Max(best, max);
        }
        return best;
    }
}
