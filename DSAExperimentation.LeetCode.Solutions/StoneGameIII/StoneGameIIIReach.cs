namespace DSAExperimentation.LeetCode.Solutions.StoneGameIII;

// The reach of StoneGameIIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. WinnerByUnmemoizedRecursion takes 222.7 ms over the 89 of the catalog's 95
// cases with a length of stoneValue up to 25 (measured 2026-11-05, Release); the larger cases run past the
// 250 ms a strategy's catalog cases are allowed.
internal static class StoneGameIIIReach
{
    private const int LargestStoneValueLength = 25;

    public static bool IsWithin(int[] stoneValue) =>
        stoneValue.Length <= LargestStoneValueLength;
}
