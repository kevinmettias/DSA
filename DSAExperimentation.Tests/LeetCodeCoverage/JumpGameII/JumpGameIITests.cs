namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameII;

public sealed class JumpGameIITests
{
    [Theory]
    [InlineData(new[] { 2, 3, 1, 1, 4 }, 2)]
    [InlineData(new[] { 2, 3, 0, 1, 4 }, 2)]
    [InlineData(new[] { 0 }, 0)]
    public void Jump_GreedyLayerExpansion_ReturnsMinimumJumps(int[] nums, int expected) => Assert.Equal(expected, Jump(nums));

    private static int Jump(int[] nums)
    {
        var jumps = 0; var currentEnd = 0; var farthest = 0;
        for (var i = 0; i < nums.Length - 1; i++)
        {
            farthest = Math.Max(farthest, i + nums[i]);
            if (i == currentEnd) { jumps++; currentEnd = farthest; }
        }
        return jumps;
    }
}
