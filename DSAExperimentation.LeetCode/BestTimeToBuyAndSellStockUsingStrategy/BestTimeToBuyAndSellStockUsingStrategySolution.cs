namespace DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockUsingStrategy;

// LeetCode 3652. Best Time to Buy and Sell Stock using Strategy: profit is
// sum(strategy[i] * prices[i]). At most one length-k window may be overwritten to
// "hold" for its first half and "sell" for its second half. Return the best
// achievable profit.
//
// Every window is scored by how much overwriting it would change the total: delta(j)
// = (sum of prices over the window's sell half) - (the window's own original
// strategy*price contribution). The answer is baseProfit + max(0, max over j of
// delta(j)) - "no modification" is always an option, so a negative best delta simply
// isn't taken. Both strategies below compute exactly that; they differ only in
// whether each window's two sums are recomputed from scratch or carried forward as
// the window slides one step, which is the same "recompute vs. maintain a running
// total" split as any other fixed-size sliding window.
internal static class BestTimeToBuyAndSellStockUsingStrategySolution
{
    // Textbook baseline: for every one of the n - k + 1 candidate windows, sum both
    // halves from scratch. O(n * k) - the arm the composed strategy below has to
    // justify itself against.
    public static long MaxProfitByBruteForceWindowSum(int[] prices, int[] strategy, int k)
    {
        var baseProfit = 0L;

        for (var i = 0; i < prices.Length; i++)
        {
            baseProfit += (long)strategy[i] * prices[i];
        }

        var bestDelta = 0L;
        var half = k / 2;

        for (var start = 0; start + k <= prices.Length; start++)
        {
            var original = 0L;
            var sellHalf = 0L;

            for (var offset = 0; offset < k; offset++)
            {
                original += (long)strategy[start + offset] * prices[start + offset];

                if (offset >= half)
                {
                    sellHalf += prices[start + offset];
                }
            }

            bestDelta = Math.Max(bestDelta, sellHalf - original);
        }

        return baseProfit + bestDelta;
    }

    // Same delta, computed with two running window sums that each move by exactly
    // one price as the window slides one step right - the original-contribution sum
    // drops prices[start] and adds prices[start + k], the sell-half sum drops
    // prices[start + half] and adds prices[start + k]. O(n) total instead of O(n * k).
    public static long MaxProfitBySlidingWindowSum(int[] prices, int[] strategy, int k)
    {
        var baseProfit = 0L;

        for (var i = 0; i < prices.Length; i++)
        {
            baseProfit += (long)strategy[i] * prices[i];
        }

        var half = k / 2;
        var original = 0L;
        var sellHalf = 0L;

        for (var i = 0; i < k; i++)
        {
            original += (long)strategy[i] * prices[i];

            if (i >= half)
            {
                sellHalf += prices[i];
            }
        }

        var bestDelta = Math.Max(0L, sellHalf - original);

        for (var start = 1; start + k <= prices.Length; start++)
        {
            var leaving = start - 1;
            var entering = start + k - 1;
            var leavingHalf = start + half - 1;

            original += (long)strategy[entering] * prices[entering] - (long)strategy[leaving] * prices[leaving];
            sellHalf += prices[entering] - prices[leavingHalf];

            bestDelta = Math.Max(bestDelta, sellHalf - original);
        }

        return baseProfit + bestDelta;
    }
}
