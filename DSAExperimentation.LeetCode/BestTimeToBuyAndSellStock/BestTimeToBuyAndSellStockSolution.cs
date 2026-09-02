namespace DSAExperimentation.LeetCode.BestTimeToBuyAndSellStock;

// LeetCode 121. Best Time to Buy and Sell Stock: one buy, one later sell, maximize
// profit (0 if no profitable trade exists - this is the single-transaction base
// case the rest of the BestTimeToBuyAndSellStock* family generalizes).
//
// Both strategies scan the same n prices; they differ only in whether every
// (buy day, sell day) pair is checked explicitly or the best profit is tracked in
// one pass against the lowest price seen so far.
internal static class BestTimeToBuyAndSellStockSolution
{
    // Textbook baseline: every buy-day/sell-day pair, O(n^2). The arm
    // MaxProfitByOnePassMinTracking has to justify itself against.
    public static int MaxProfitByBruteForce(int[] prices)
    {
        var best = 0;

        for (var buy = 0; buy < prices.Length; buy++)
        {
            for (var sell = buy + 1; sell < prices.Length; sell++)
            {
                best = Math.Max(best, prices[sell] - prices[buy]);
            }
        }

        return best;
    }

    // One pass: track the lowest price seen so far and the profit selling today
    // would realize against it.
    public static int MaxProfitByOnePassMinTracking(int[] prices)
    {
        var min = int.MaxValue;
        var best = 0;

        foreach (var price in prices)
        {
            min = Math.Min(min, price);
            best = Math.Max(best, price - min);
        }

        return best;
    }
}
