using DSAExperimentation.LeetCode.HouseRobber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobber;

// Harness only: both strategies live in HouseRobberSolution and are asserted
// against LeetCode's published examples, plus a couple of thin edge cases the
// original test never covered.
public sealed partial class HouseRobberTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { new[] { 1, 2, 3, 1 }, 4 },
            { new[] { 2, 7, 9, 3, 1 }, 12 },
            { new[] { 5 }, 5 },
            { new[] { 2, 1, 1, 2 }, 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobByMemoizedRecursion_LeetCodeExamples_ReturnsBestNonAdjacentSum(int[] nums, int expected) =>
        Assert.Equal(expected, HouseRobberSolution.RobByMemoizedRecursion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RobByIterativeRollingTotals_LeetCodeExamples_ReturnsBestNonAdjacentSum(int[] nums, int expected) =>
        Assert.Equal(expected, HouseRobberSolution.RobByIterativeRollingTotals(nums));

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they name the same haul on every example - not merely that
    // each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void Rob_AgreeOnEveryExample(int[] nums, int expected) =>
        Assert.Equal(
            HouseRobberSolution.RobByMemoizedRecursion(nums),
            HouseRobberSolution.RobByIterativeRollingTotals(nums));
}
