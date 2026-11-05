namespace DSAExperimentation.LeetCode.Solutions.StoneGameVIII;

// The reach of StoneGameVIIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. MaxScoreDifferenceByUnmemoizedRecursion takes 92.3 ms over the 71 of the
// catalog's 73 cases with a length of stones up to 25 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class StoneGameVIIIReach
{
    private const int LargestStonesLength = 25;

    public static bool IsWithin(int[] stones) =>
        stones.Length <= LargestStonesLength;
}
