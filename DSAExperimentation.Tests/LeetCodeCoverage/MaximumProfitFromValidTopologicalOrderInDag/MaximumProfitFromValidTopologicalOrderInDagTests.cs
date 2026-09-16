using DSAExperimentation.LeetCode.MaximumProfitFromValidTopologicalOrderInDag;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProfitFromValidTopologicalOrderInDag;

// Harness only. PrecedenceMasks reduces LC 3530's DAG to one predecessor
// bitmask per node and both strategies are
// MaximumProfitFromValidTopologicalOrderInDagSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class MaximumProfitFromValidTopologicalOrderInDagTests
{
    public static TheoryData<int, int[][], int[], long> Examples =>
        new()
        {
            { 2, [[0, 1]], [2, 3], 8 },
            { 3, [[0, 1], [0, 2]], [1, 6, 3], 25 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByBacktracking_LeetCodeExamples_ReturnsMaximumAchievableProfit(
        int nodeCount, int[][] edges, int[] score, long expected)
    {
        var actual = MaximumProfitFromValidTopologicalOrderInDagSolution.MaxProfitByBacktracking(
            nodeCount, edges, score);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxProfitByBitmaskMemoization_LeetCodeExamples_ReturnsMaximumAchievableProfit(
        int nodeCount, int[][] edges, int[] score, long expected)
    {
        var actual = MaximumProfitFromValidTopologicalOrderInDagSolution.MaxProfitByBitmaskMemoization(
            nodeCount, edges, score);

        Assert.Equal(expected, actual);
    }
}
