namespace DSAExperimentation.LeetCode.Solutions.SumGame;

// The reach of SumGameSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. CanAliceWinByBruteForceRecursion takes 161.8 ms over the 23 of the
// catalog's 168 cases with a length of num up to 9 (measured 2026-11-05, Release); the larger cases run past
// the 250 ms a strategy's catalog cases are allowed.
internal static class SumGameReach
{
    private const int LargestNumLength = 9;

    public static bool IsWithin(string num) =>
        num.Length <= LargestNumLength;
}
