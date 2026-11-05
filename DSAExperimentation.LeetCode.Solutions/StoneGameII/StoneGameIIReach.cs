namespace DSAExperimentation.LeetCode.Solutions.StoneGameII;

// The reach of StoneGameIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. MaxStonesByUnmemoizedRecursion takes 11.6 ms over the 67 of the catalog's
// 91 cases with a length of piles up to 25 (measured 2026-11-05, Release); the larger cases run past the 250
// ms a strategy's catalog cases are allowed.
internal static class StoneGameIIReach
{
    private const int LargestPilesLength = 25;

    public static bool IsWithin(int[] piles) =>
        piles.Length <= LargestPilesLength;
}
