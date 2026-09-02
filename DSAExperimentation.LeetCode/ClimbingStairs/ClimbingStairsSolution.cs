using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.ClimbingStairs;

// LeetCode 70. Climbing Stairs: ways(n) = ways(n-1) + ways(n-2), the same shape as
// Fibonacci - natural-looking recursion via this repo's Memoizer, no hand-rolled
// cache.
internal static class ClimbingStairsSolution
{
    public static int CountWaysByMemoizedRecurrence(int stepCount) =>
        Memoizer.Memoize<int, int>(
            stepCount, (n, ways) => n <= 1 ? 1 : ways(n - 1) + ways(n - 2));
}
