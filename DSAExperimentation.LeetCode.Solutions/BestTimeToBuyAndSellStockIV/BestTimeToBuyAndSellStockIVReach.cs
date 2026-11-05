namespace DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStockIV;

// The reach of BestTimeToBuyAndSellStockIVSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. MaxProfitByBruteForce takes 164.4 ms over the 92 of the
// catalog's 95 cases with a length of prices up to 28 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class BestTimeToBuyAndSellStockIVReach
{
    private const int LargestPricesLength = 28;

    public static bool IsWithin(int transactionBudget, int[] prices) =>
        prices.Length <= LargestPricesLength;
}
