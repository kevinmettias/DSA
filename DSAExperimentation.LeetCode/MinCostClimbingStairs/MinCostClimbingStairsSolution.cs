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
    public static int MinCostByNaiveRecursive(int[] cost)
    {
        var fromFirstStep = CostFrom(0, cost);
        var fromSecondStep = CostFrom(1, cost);
        return Math.Min(fromFirstStep, fromSecondStep);
    }

    // This repo's own top-down DP: Memoizer over the exact recurrence above, with
    // the virtual start state (-1) folding "start at 0 or 1" into one shared cache.
    public static int MinCostByMemoizedRecurrence(int[] cost) =>
        Memoizer.Memoize<int, int>(-1, new CheapestWayFromStep(cost));

    private static int MinCostFromStep(int step, int[] cost, IRecurrence<int, int> rest)
    {
        if (step < 0)
        {
            var fromFirstStep = rest.Replay(0, rest);
            var fromSecondStep = rest.Replay(1, rest);

            return Math.Min(fromFirstStep, fromSecondStep);
        }

        if (step >= cost.Length)
        {
            return 0;
        }

        var afterOneStep = rest.Replay(step + 1, rest);
        var afterTwoSteps = rest.Replay(step + TwoStepClimb, rest);

        return cost[step] + Math.Min(afterOneStep, afterTwoSteps);
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

    private static int CostFrom(int step, int[] cost) =>
        step >= cost.Length
            ? 0
            : CostOfStep(step, cost);

    // The step's own cost plus the cheaper of the two ways on from it.
    private static int CostOfStep(int step, int[] cost)
    {
        var afterOneStep = CostFrom(step + 1, cost);
        var afterTwoSteps = CostFrom(step + TwoStepClimb, cost);
        return cost[step] + Math.Min(afterOneStep, afterTwoSteps);
    }

    // The climbing rule, named: from a step, the cheapest way to the top is that step's own
    // cost plus the better of the one-step and two-step continuations - and the virtual
    // start state below the first step simply takes the better of the two entry points. The
    // cost array is fixed for the whole walk and arrives once through the primary
    // constructor; `rest` is the memo run's own handle on this rule.
    private sealed class CheapestWayFromStep(int[] cost) : IRecurrence<int, int>
    {
        /// <inheritdoc/>
        public int Replay(int step, IRecurrence<int, int> rest) => MinCostFromStep(step, cost, rest);
    }
}
