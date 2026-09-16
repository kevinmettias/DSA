using DSAExperimentation.LeetCode.ClimbingStairsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClimbingStairsII;

// Harness only. ClimbingStairsIISolution owns both the brute-force baseline and the
// memoized recurrence; this file pins them to LeetCode's three published examples.
public sealed class ClimbingStairsIITests
{
    public static TheoryData<int, int[], long> Examples =>
        new()
        {
            { 4, [1, 2, 3, 4], 13L },
            { 4, [5, 1, 6, 2], 11L },
            { 3, [9, 8, 3], 12L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByBruteForce_LeetCodeExamples_ReturnsMinimumTotalCost(
        int n, int[] costs, long expected)
    {
        var minCost = ClimbingStairsIISolution.MinCostByBruteForce(n, costs);

        Assert.Equal(expected, minCost);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinCostByMemoizedRecurrence_LeetCodeExamples_ReturnsMinimumTotalCost(
        int n, int[] costs, long expected)
    {
        var minCost = ClimbingStairsIISolution.MinCostByMemoizedRecurrence(n, costs);

        Assert.Equal(expected, minCost);
    }
}
