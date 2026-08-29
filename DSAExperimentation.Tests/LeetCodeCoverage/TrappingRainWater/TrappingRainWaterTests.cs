namespace DSAExperimentation.Tests.LeetCodeCoverage.TrappingRainWater;

public sealed class TrappingRainWaterTests
{
    [Theory]
    [InlineData(new[] { 0, 1, 0, 2, 1, 0, 1, 3, 2, 1, 2, 1 }, 6)]
    [InlineData(new[] { 4, 2, 0, 3, 2, 5 }, 9)]
    [InlineData(new[] { 1, 2, 3 }, 0)]
    public void Trap_UsesTwoBoundaryScan_ReturnsExpectedWater(int[] height, int expected) => Assert.Equal(expected, Trap(height));

    private static int Trap(int[] height)
    {
        var left = 0; var right = height.Length - 1; var leftMax = 0; var rightMax = 0; var water = 0;
        while (left < right)
        {
            if (height[left] < height[right]) { leftMax = Math.Max(leftMax, height[left]); water += leftMax - height[left]; left++; }
            else { rightMax = Math.Max(rightMax, height[right]); water += rightMax - height[right]; right--; }
        }
        return water;
    }
}
