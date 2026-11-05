namespace DSAExperimentation.LeetCode.Solutions.ChalkboardXorGame;

// The reach of ChalkboardXorGameSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. CanAliceWinByBruteForceRecursion takes 68 ms over the 46 of the
// catalog's 102 cases with a length of nums up to 13 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class ChalkboardXorGameReach
{
    private const int LargestNumsLength = 13;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
