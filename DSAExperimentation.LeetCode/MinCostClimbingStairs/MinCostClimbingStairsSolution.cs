using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinCostClimbingStairs;

// LeetCode 746. Min Cost Climbing Stairs: minCost(i) = cost[i] + min(minCost(i+1),
// minCost(i+2)), the same recurrence shape ClimbingStairsSolution uses for LC 70. A
// virtual start state (-1) represents "before the first step," folding the free
// choice of starting at index 0 or 1 into Math.Min(minCost(0), minCost(1)) inside
// one shared cache, instead of two separate top-level calls.
internal static class MinCostClimbingStairsSolution
{
    // LC 746 allows climbing 1 or 2 steps at a time.
    private const int TwoStepClimb = 2;

    // The textbook answer: the recurrence re-derived from both starting steps with
    // no cache - O(2^n), deliberately written without this repo's Memoizer. It is
    // the arm the memoized and iterative strategies below have to justify
    // themselves against.
    public static int MinCostByNaiveRecursive(int[] cost) =>
        Math.Min(CostFrom(0, cost), CostFrom(1, cost));

    private static int CostFrom(int step, int[] cost) =>
        step >= cost.Length
            ? 0
            : cost[step] + Math.Min(CostFrom(step + 1, cost), CostFrom(step + TwoStepClimb, cost));

    // This repo's own top-down DP: Memoizer over the exact recurrence above, with
    // the virtual start state (-1) folding "start at 0 or 1" into one shared cache.
    public static int MinCostByMemoizedRecurrence(int[] cost) =>
        Memoizer.Memoize<int, int>(-1, (step, minCostFrom) => MinCostFromStep(step, cost, minCostFrom));

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

        return cost[step] + Math.Min(minCostFrom(step + 1), minCostFrom(step + TwoStepClimb));
    }

    // The O(n)-time O(1)-space answer neither of the other two even needs to beat.
    public static int MinCostByIterativeConstantSpace(int[] cost)
    {
        var (twoBack, oneBack) = (0, 0);

        for (var i = TwoStepClimb; i <= cost.Length; i++)
        {
            var current = Math.Min(oneBack + cost[i - 1], twoBack + cost[i - TwoStepClimb]);
            (twoBack, oneBack) = (oneBack, current);
        }

        return oneBack;
    }
}
