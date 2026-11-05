using DSAExperimentation.LeetCode.Solutions.HouseRobberII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.HouseRobberII;

// Harness only: both strategies live in HouseRobberIISolution and are asserted
// against the single-house and two-house edge
// cases the original test never covered (the circular wrap-around only bites once
// there are at least two houses to skip between).
public sealed partial class HouseRobberIISolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { new[] { 5 }, 5 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void RobByMemoizedRecursion_Cases_ReturnsCircularBest(int[] nums, int expected) =>
        Assert.Equal(expected, HouseRobberIISolution.RobByMemoizedRecursion(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void RobByIterativeTwoPass_Cases_ReturnsCircularBest(int[] nums, int expected) =>
        Assert.Equal(expected, HouseRobberIISolution.RobByIterativeTwoPass(nums));
}
