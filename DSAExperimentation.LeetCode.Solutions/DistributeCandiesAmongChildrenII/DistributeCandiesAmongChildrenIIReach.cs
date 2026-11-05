namespace DSAExperimentation.LeetCode.Solutions.DistributeCandiesAmongChildrenII;

// The reach of DistributeCandiesAmongChildrenIISolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. CountWaysByBruteForce takes 196.5 ms over the 77 of the
// catalog's 90 cases with a value of limit up to 12,345 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class DistributeCandiesAmongChildrenIIReach
{
    private const int LargestLimit = 12_345;

    public static bool IsWithin(int candyCount, int limit) =>
        limit <= LargestLimit;
}
