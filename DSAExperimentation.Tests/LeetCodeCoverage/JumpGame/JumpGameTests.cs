namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGame;

public sealed class JumpGameTests
{
    [Theory]
    [InlineData(new[] { 2, 3, 1, 1, 4 }, true)]
    [InlineData(new[] { 3, 2, 1, 0, 4 }, false)]
    [InlineData(new[] { 0 }, true)]
    public void CanJump_GreedyReachScan_ReturnsReachability(int[] nums, bool expected) => Assert.Equal(expected, CanJump(nums));

    private static bool CanJump(int[] nums)
    {
        var reach = 0;
        for (var i = 0; i < nums.Length && i <= reach; i++) reach = Math.Max(reach, i + nums[i]);
        return reach >= nums.Length - 1;
    }
}
