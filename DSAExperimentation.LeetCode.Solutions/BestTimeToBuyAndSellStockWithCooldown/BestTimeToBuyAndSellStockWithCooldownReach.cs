namespace DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStockWithCooldown;

// The reach of BestTimeToBuyAndSellStockWithCooldownSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. MaxProfitByUnmemoizedRecursion takes 192.5 ms over
// the 96 of the catalog's 103 cases with a length of prices up to 28 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class BestTimeToBuyAndSellStockWithCooldownReach
{
    private const int LargestPricesLength = 28;

    public static bool IsWithin(int[] prices) =>
        prices.Length <= LargestPricesLength;
}
