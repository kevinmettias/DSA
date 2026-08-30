using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinCostClimbingStairs;

// LeetCode 746. Min Cost Climbing Stairs: minCost(i) = cost[i] + min(minCost(i+1),
// minCost(i+2)), the same top-down recurrence shape ClimbingStairsTests already uses
// for LC 70, via this repo's own Memoizer. A virtual start state (-1) represents
// "before the first step," folding the free choice of starting at index 0 or 1 into
// Math.Min(minCost(0), minCost(1)) inside one shared cache, instead of two separate
// top-level Memoize calls.
public sealed partial class MinCostClimbingStairsTests
{
    [Fact]
    public void MinCost_ThreeStepExample_ReturnsCheaperOfTheTwoStartingSteps()
    {
        int[] cost = [10, 15, 20];

        var minCost = MinCostClimbingStairs(cost);

        Assert.Equal(15, minCost);
    }

    [Fact]
    public void MinCost_TenStepExample_ReturnsSixByAlternatingCheapSteps()
    {
        int[] cost = [1, 100, 1, 1, 1, 100, 1, 1, 100, 1];

        var minCost = MinCostClimbingStairs(cost);

        Assert.Equal(6, minCost);
    }

    private static int MinCostClimbingStairs(int[] cost)
        => Memoizer.Memoize<int, int>(-1, (step, minCostFrom) => MinCostFromStep(step, cost, minCostFrom));

    private static int MinCostFromStep(int step, int[] cost, Func<int, int> minCostFrom)
    {
        if (step < 0)
        {
            return Math.Min(minCostFrom(0), minCostFrom(1));
        }

        if (step >= cost.Length)
        {
            return 0;
        }

        return cost[step] + Math.Min(minCostFrom(step + 1), minCostFrom(step + 2));
    }
}
