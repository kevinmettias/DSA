namespace DSAExperimentation.LeetCode.Solutions.BestTimeToBuyAndSellStockWithTransactionFee;

// The reach of BestTimeToBuyAndSellStockWithTransactionFeeSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. MaxProfitByUnmemoizedRecursion takes 23.8
// ms over the 77 of the catalog's 94 cases with a length of prices up to 20 and a value of fee up to 100
// (measured 2026-11-05, Release); the larger cases run past the 250 ms a strategy's catalog cases are
// allowed.
internal static class BestTimeToBuyAndSellStockWithTransactionFeeReach
{
    private const int LargestPricesLength = 20;
    private const int LargestFee = 100;

    public static bool IsWithin(int[] prices, int fee) =>
        prices.Length <= LargestPricesLength && fee <= LargestFee;
}
