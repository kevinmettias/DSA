using DSAExperimentation.LeetCode.MinCostClimbingStairs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinCostClimbingStairs;

// Harness only. MinCostClimbingStairsSolution owns all three strategies; this file
// pins each of them to LeetCode's published examples.
public sealed partial class MinCostClimbingStairsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [10, 15, 20], 15 },
            { [1, 100, 1, 1, 1, 100, 1, 1, 100, 1], 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByNaiveRecursive_LeetCodeExamples_ReturnsCheapestClimbCost(
        int[] cost, int expected) =>
        Assert.Equal(expected, MinCostClimbingStairsSolution.MinCostByNaiveRecursive(cost));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByMemoizedRecurrence_LeetCodeExamples_ReturnsCheapestClimbCost(
        int[] cost, int expected) =>
        Assert.Equal(expected, MinCostClimbingStairsSolution.MinCostByMemoizedRecurrence(cost));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByIterativeConstantSpace_LeetCodeExamples_ReturnsCheapestClimbCost(
        int[] cost, int expected) =>
        Assert.Equal(expected, MinCostClimbingStairsSolution.MinCostByIterativeConstantSpace(cost));
}
