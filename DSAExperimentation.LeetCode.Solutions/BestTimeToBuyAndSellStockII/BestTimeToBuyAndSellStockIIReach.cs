namespace DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStockII;

// The reach of BestTimeToBuyAndSellStockIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. MaxProfitByBruteForce takes 135.6 ms over the 94 of the
// catalog's 120 cases with a length of prices up to 20 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class BestTimeToBuyAndSellStockIIReach
{
    private const int LargestPricesLength = 20;

    public static bool IsWithin(int[] prices) =>
        prices.Length <= LargestPricesLength;
}
