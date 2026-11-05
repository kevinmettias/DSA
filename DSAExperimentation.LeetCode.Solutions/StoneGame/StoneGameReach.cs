namespace DSAExperimentation.LeetCode.Solutions.StoneGame;

// The reach of StoneGameSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. CanAliceWinByUnmemoizedRecursion takes 141.8 ms over the 104 of the
// catalog's 106 cases with a length of piles up to 24 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class StoneGameReach
{
    private const int LargestPilesLength = 24;

    public static bool IsWithin(int[] piles) =>
        piles.Length <= LargestPilesLength;
}
