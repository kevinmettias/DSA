using DSAExperimentation.LeetCode.HouseRobberII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobberII;

// Harness only: both strategies live in HouseRobberIISolution and are asserted
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobByIterativeTwoPass_LeetCodeExamples_ReturnsCircularBest(int[] nums, int expected) =>
        Assert.Equal(expected, HouseRobberIISolution.RobByIterativeTwoPass(nums));

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they name the same haul on every example - not merely that
    // each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void Rob_AgreeOnEveryExample(int[] nums, int expected) =>
        Assert.Equal(
            HouseRobberIISolution.RobByMemoizedRecursion(nums),
            HouseRobberIISolution.RobByIterativeTwoPass(nums));
}
