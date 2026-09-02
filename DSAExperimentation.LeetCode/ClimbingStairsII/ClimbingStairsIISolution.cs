using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.ClimbingStairsII;

// LeetCode 3693. Climbing Stairs II: minCost(j) = min over reachable predecessors
// i in {j-1, j-2, j-3} of minCost(i) + costs[j-1] + (j-i)^2, minCost(0) = 0. The
// naive baseline re-derives this with plain unmemoized recursion - exponential,
// since every step re-explores every path to it from scratch. The composed
// strategy dogfoods this repo's own Memoizer over the identical recurrence, the
// same "natural-looking recursion, no hand-rolled cache" shape ClimbingStairsSolution
// (LC 70) and MinCostClimbingStairsTests (LC 746) already use.
internal static class ClimbingStairsIISolution
{
    private const int MaxJump = 3;

    // The textbook answer: recompute every subpath from scratch, no cache -
    // deliberately written without this repo's primitives, the arm the composed
    // strategy below has to justify itself against.
    public static long MinCostByBruteForce(int n, int[] costs) => MinCostFrom(n, costs);

    private static long MinCostFrom(int step, int[] costs)
    {
        if (step == 0)
        {
            return 0;
        }

        var best = long.MaxValue;

        for (var jump = 1; jump <= Math.Min(MaxJump, step); jump++)
        {
            var origin = step - jump;
            var candidate = MinCostFrom(origin, costs) + costs[step - 1] + (long)jump * jump;
            best = Math.Min(best, candidate);
        }

        return best;
    }

    // This repo's own Memoizer over the identical recurrence - natural-looking
    // recursion via a shared cache instead of a hand-rolled dp[] array.
    public static long MinCostByMemoizedRecurrence(int n, int[] costs) =>
        Memoizer.Memoize<int, long>(n, (step, minCostTo) => MinCostToStep(step, costs, minCostTo));

    private static long MinCostToStep(int step, int[] costs, Func<int, long> minCostTo)
    {
        if (step == 0)
        {
            return 0;
        }

        var best = long.MaxValue;

        for (var jump = 1; jump <= Math.Min(MaxJump, step); jump++)
        {
            var origin = step - jump;
            var candidate = minCostTo(origin) + costs[step - 1] + (long)jump * jump;
            best = Math.Min(best, candidate);
        }

        return best;
    }
}
