namespace DSAExperimentation.LeetCode.Solutions.FindNumberOfWaysToReachTheKthStair;

// The reach of FindNumberOfWaysToReachTheKthStairSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. WaysByBruteRecursion takes 217.1 ms over the 59 of
// the catalog's 78 cases with a value of targetStair up to 5,000,000 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class FindNumberOfWaysToReachTheKthStairReach
{
    private const int LargestTargetStair = 5_000_000;

    public static bool IsWithin(int targetStair) =>
        targetStair <= LargestTargetStair;
}
