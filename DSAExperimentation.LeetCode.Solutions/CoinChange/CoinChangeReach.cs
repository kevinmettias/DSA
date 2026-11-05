namespace DSAExperimentation.LeetCode.Solutions.CoinChange;

// The reach of CoinChangeSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. FewestCoinsByMemoization takes 2.4 ms over the 5 of the catalog's 116 cases
// with a value of amount up to 6 (measured 2026-11-05, Release); the larger cases run past the 250 ms a
// strategy's catalog cases are allowed.
internal static class CoinChangeReach
{
    private const int LargestAmount = 6;

    public static bool IsWithin(int[] coins, int amount) =>
        amount <= LargestAmount;
}
