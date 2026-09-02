namespace DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockII;

// LeetCode 122. Best Time to Buy and Sell Stock II: unlimited transactions, but
// never hold more than one share at a time (must sell before buying again). Unlike
// LC 121's single trade, every local upswing can be captured on its own, so the
// two strategies differ in whether that fact is exploited directly or discovered
// by exhaustively trying every buy/sell decision.
internal static class BestTimeToBuyAndSellStockIISolution
{
    // Textbook baseline: an unmemoized two-way choice (buy-or-skip when flat,
    // sell-or-skip when holding) at every day, O(2^n). The arm
    // MaxProfitByGreedyAscent has to justify itself against.
    public static int MaxProfitByBruteForce(int[] prices) => ProfitFrom(0, holding: false, prices);

    private static int ProfitFrom(int day, bool holding, int[] prices)
    {
        if (day == prices.Length)
        {
            return 0;
        }

        var skip = ProfitFrom(day + 1, holding, prices);

        return holding
            ? Math.Max(skip, prices[day] + ProfitFrom(day + 1, holding: false, prices))
            : Math.Max(skip, -prices[day] + ProfitFrom(day + 1, holding: true, prices));
    }

    // Every consecutive-day price increase can be captured as its own
    // buy-yesterday/sell-today transaction, so the maximum total profit is just
    // the sum of every positive day-over-day delta - one pass, no state to carry
    // beyond the previous day's price.
    public static int MaxProfitByGreedyAscent(int[] prices)
    {
        var profit = 0;

        for (var i = 1; i < prices.Length; i++)
        {
            if (prices[i] > prices[i - 1])
            {
                profit += prices[i] - prices[i - 1];
            }
        }

        return profit;
    }
}
