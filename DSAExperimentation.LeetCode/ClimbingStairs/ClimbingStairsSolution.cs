using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.ClimbingStairs;

// LeetCode 70. Climbing Stairs: ways(n) = ways(n-1) + ways(n-2), the same shape as
// Fibonacci - natural-looking recursion via this repo's Memoizer, no hand-rolled
// cache.
internal static class ClimbingStairsSolution
{
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
