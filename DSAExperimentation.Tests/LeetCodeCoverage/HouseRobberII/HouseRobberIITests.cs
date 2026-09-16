using DSAExperimentation.LeetCode.HouseRobberII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobberII;

// Harness only: the one strategy lives in HouseRobberIISolution and is asserted
// against LeetCode's published examples, plus the single-house and two-house edge
// cases the original test never covered (the circular wrap-around only bites once
// there are at least two houses to skip between).
public sealed partial class HouseRobberIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { new[] { 2, 3, 2 }, 3 },
            { new[] { 1, 2, 3, 1 }, 4 },
            { new[] { 1, 2, 3 }, 3 },
            { new[] { 5 }, 5 },
            { new[] { 1, 2 }, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobByMemoizedRecursion_LeetCodeExamples_ReturnsCircularBest(int[] nums, int expected) =>
        Assert.Equal(expected, HouseRobberIISolution.RobByMemoizedRecursion(nums));
}
