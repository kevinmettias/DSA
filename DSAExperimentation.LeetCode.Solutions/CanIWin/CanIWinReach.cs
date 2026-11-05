namespace DSAExperimentation.LeetCode.Solutions.CanIWin;

// The reach of CanIWinSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. CanWinByBruteForceRecursion takes 197.7 ms over the 39 of the catalog's 75
// cases with a value of desiredTotal up to 75 (measured 2026-11-05, Release); the larger cases run past the
// 250 ms a strategy's catalog cases are allowed.
internal static class CanIWinReach
{
    private const int LargestDesiredTotal = 75;

    public static bool IsWithin(int maxChoosableInteger, int desiredTotal) =>
        desiredTotal <= LargestDesiredTotal;
}
