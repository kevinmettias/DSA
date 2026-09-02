using DSAExperimentation.LeetCode.HouseRobber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HouseRobber;

// Harness only: the one strategy lives in HouseRobberSolution and is asserted
// against LeetCode's published examples, plus a couple of thin edge cases the
// original test never covered.
public sealed class HouseRobberTests
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
}
