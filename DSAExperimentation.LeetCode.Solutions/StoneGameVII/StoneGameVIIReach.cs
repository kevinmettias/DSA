namespace DSAExperimentation.LeetCode.Solutions.StoneGameVII;

// The reach of StoneGameVIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. MaxScoreDifferenceByMemoizedRecursion takes 2.9 ms over the 32 of the
// catalog's 93 cases with a length of stones up to 13 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class StoneGameVIIReach
{
    private const int LargestStonesLength = 13;

    public static bool IsWithin(int[] stones) =>
        stones.Length <= LargestStonesLength;
}
