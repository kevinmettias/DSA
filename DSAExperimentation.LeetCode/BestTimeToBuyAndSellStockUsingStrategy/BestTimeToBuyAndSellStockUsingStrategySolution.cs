namespace DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockUsingStrategy;

// LeetCode 3652. Best Time to Buy and Sell Stock using Strategy: profit is
// sum(strategy[i] * prices[i]). At most one window of `windowSize` consecutive days
// may be overwritten to "hold" for its first half and "sell" for its second half.
// Return the best achievable profit.
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
    public static long MaxProfitByBruteForceWindowSum(int[] prices, int[] strategy, int windowSize)
    {
        var baseProfit = BaseProfit(prices, strategy);
        var bestDelta = 0L;
        var half = windowSize / 2;

        for (var start = 0; start + windowSize <= prices.Length; start++)
        {
            var delta = WindowDelta((prices, strategy), (start, windowSize), half);
            bestDelta = Math.Max(bestDelta, delta);
        }

        return baseProfit + bestDelta;
    }

    // What overwriting one window with "hold then sell" would do to the total: the price sum
    // over its sell half, less the strategy*price contribution the window already had. Both
    // halves are recomputed from scratch, which is the arm the sliding version below avoids.
    private static long WindowDelta(
        (int[] Prices, int[] Strategy) market, (int Start, int WindowSize) window, int half)
    {
        var (prices, strategy) = market;
        var (start, windowSize) = window;
        var original = 0L;
        var sellHalf = 0L;

        for (var offset = 0; offset < windowSize; offset++)
        {
            original += (long)strategy[start + offset] * prices[start + offset];

            if (offset >= half)
            {
                sellHalf += prices[start + offset];
            }
        }

        return sellHalf - original;
    }

    // Same delta, computed with two running window sums that each move by exactly
    // one price as the window slides one step right - the original-contribution sum
    // drops prices[start] and adds prices[start + windowSize], the sell-half sum drops
    // prices[start + half] and adds prices[start + windowSize]. O(n) total instead of O(n * k).
    public static long MaxProfitBySlidingWindowSum(int[] prices, int[] strategy, int windowSize)
    {
        var baseProfit = BaseProfit(prices, strategy);
        var half = windowSize / 2;
        var (original, sellHalf) = InitialWindowSums((prices, strategy), windowSize, half);
        var bestDelta = Math.Max(0L, sellHalf - original);

        for (var start = 1; start + windowSize <= prices.Length; start++)
        {
            (original, sellHalf) = SlideWindow(
                (prices, strategy), (start, windowSize), half, (original, sellHalf));
            bestDelta = Math.Max(bestDelta, sellHalf - original);
        }

        return baseProfit + bestDelta;
    }

    // The first window's two sums, computed from scratch: its strategy*price contribution
    // over the whole window, and the price sum over its second half alone.
    private static (long Original, long SellHalf) InitialWindowSums(
        (int[] Prices, int[] Strategy) market, int windowSize, int half)
    {
        var (prices, strategy) = market;
        var original = 0L;
        var sellHalf = 0L;

        for (var i = 0; i < windowSize; i++)
        {
            original += (long)strategy[i] * prices[i];

            if (i >= half)
            {
                sellHalf += prices[i];
            }
        }

        return (original, sellHalf);
    }

    // Carry both running sums one step right: the original-contribution sum drops the price
    // leaving the window and adds the one entering it, and the sell-half sum does the same.
    private static (long Original, long SellHalf) SlideWindow(
        (int[] Prices, int[] Strategy) market,
        (int Start, int WindowSize) window,
        int half,
        (long Original, long SellHalf) running)
    {
        var (prices, strategy) = market;
        var (start, windowSize) = window;
        var (original, sellHalf) = running;
        var leaving = start - 1;
        var entering = start + windowSize - 1;
        var leavingHalf = start + half - 1;

        original += (long)strategy[entering] * prices[entering] - (long)strategy[leaving] * prices[leaving];
        sellHalf += prices[entering] - prices[leavingHalf];

        return (original, sellHalf);
    }

    // The profit the unmodified strategy already earns - the floor both strategies measure
    // their delta against.
    private static long BaseProfit(int[] prices, int[] strategy)
    {
        var baseProfit = 0L;

        for (var i = 0; i < prices.Length; i++)
        {
            baseProfit += (long)strategy[i] * prices[i];
        }

        return baseProfit;
    }
}
