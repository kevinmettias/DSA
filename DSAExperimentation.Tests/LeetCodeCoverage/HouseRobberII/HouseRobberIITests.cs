using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobberII;

public sealed partial class HouseRobberIITests
{
    [Theory]
    [InlineData(new[] { 2, 3, 2 }, 3)]
    [InlineData(new[] { 1, 2, 3, 1 }, 4)]
    public void Rob_Examples_ReturnsCircularBest(int[] nums, int expected)
        => Assert.Equal(expected, Rob(nums));

    private static int Rob(int[] nums)
    {
        if (nums.Length == 1)
        {
            return nums[0];
        }

        var robExcludingLastHouse = RobRange(nums, 0, nums.Length - 2);
        var robExcludingFirstHouse = RobRange(nums, 1, nums.Length - 1);
        return Math.Max(robExcludingLastHouse, robExcludingFirstHouse);
    }

    private static int RobRange(int[] nums, int start, int end)
        => Memoizer.Memoize<int, int>(start, (i, rob) => i > end ? 0 : Math.Max(rob(i + 1), nums[i] + rob(i + 2)));
}
