namespace DSAExperimentation.LeetCode.Solutions.MinCostClimbingStairs;

// The reach of MinCostClimbingStairsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. MinCostByNaiveRecursive takes 110 ms over the 87 of the catalog's
// 95 cases with a length of cost up to 31 (measured 2026-11-05, Release); the larger cases run past the 250
// ms a strategy's catalog cases are allowed.
internal static class MinCostClimbingStairsReach
{
    private const int LargestCostLength = 31;

    public static bool IsWithin(int[] cost) =>
        cost.Length <= LargestCostLength;
}
