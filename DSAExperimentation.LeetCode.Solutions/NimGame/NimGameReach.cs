namespace DSAExperimentation.LeetCode.Solutions.NimGame;

// The reach of NimGameSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. CanWinByMemoizedRecursion takes 2.8 ms over the 61 of the catalog's 114
// cases with a value of stoneCount up to 1,000 (measured 2026-11-05, Release); the larger cases run past the
// 250 ms a strategy's catalog cases are allowed.
internal static class NimGameReach
{
    private const int LargestStoneCount = 1000;

    public static bool IsWithin(int stoneCount) =>
        stoneCount <= LargestStoneCount;
}
