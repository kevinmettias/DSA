using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.ClimbingStairs;

// LeetCode 70. Climbing Stairs: ways(n) = ways(n-1) + ways(n-2), the same shape as
// Fibonacci - natural-looking recursion via this repo's Memoizer, no hand-rolled
// cache.
internal static class ClimbingStairsSolution
{
    // The textbook arm the memoized recurrence is measured against: the identical
    // recurrence rolled forward from two running totals, so it pays neither
    // Memoizer's dictionary probe per state nor a call stack as deep as stepCount.
    public static int CountWaysByIterativeRollingTotals(int stepCount)
    {
        var previous = 1; // ways(0)
        var current = 1;  // ways(1)

        for (var step = 2; step <= stepCount; step++)
        {
            (previous, current) = (current, previous + current);
        }

        return current;
    }

    public static int CountWaysByMemoizedRecurrence(int stepCount) =>
        Memoizer.Memoize<int, int>(stepCount, new WaysFromPreviousTwoSteps());

    // The recurrence itself, named: the ways up to the two steps below this one,
    // summed. The base case plus that sum is the whole rule.
    private sealed class WaysFromPreviousTwoSteps : IRecurrence<int, int>
    {
        public int Replay(int state, IRecurrence<int, int> rest)
        {
            if (state <= 1)
            {
                return 1;
            }

            return rest.Replay(state - 1, rest) + rest.Replay(state - 2, rest);
        }
    }
}
