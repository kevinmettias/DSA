namespace DSAExperimentation.LeetCode.Solutions.StoneGameIV;

// The reach of StoneGameIVSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. CanAliceWinByUnmemoizedRecursion takes 119.4 ms over the 14 of the
// catalog's 24 cases with a value of stoneCount up to 55 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class StoneGameIVReach
{
    private const int LargestStoneCount = 55;

    public static bool IsWithin(int stoneCount) =>
        stoneCount <= LargestStoneCount;
}
