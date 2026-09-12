using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockWithTransactionFee;

// LeetCode 714. Best Time to Buy and Sell Stock with Transaction Fee: the same
// (day, holding) state-machine DP shape this repo's LC 309 (Best Time to Buy and
// Sell Stock with Cooldown) already establishes, just with the "next buy skipped"
// cooldown rule swapped out for "subtract fee once per completed sale" - the sell
// transition pays fee instead of advancing an extra day.
internal static class BestTimeToBuyAndSellStockWithTransactionFeeSolution
{
    // Plain un-memoized recursion over (day, holding) - exponential, since the
    // same state recurs along many different buy/sell/rest paths. Deliberately
    // written without this repo's primitives - it is the arm the composed
    // solution below has to justify itself against.
    public static int MaxProfitByUnmemoizedRecursion(int[] prices, int fee) =>
        ProfitFrom(prices, fee, 0, false);

    private static int ProfitFrom(int[] prices, int fee, int day, bool holding)
    {
        if (day >= prices.Length)
        {
            return 0;
        }

        if (holding)
        {
            var sell = prices[day] - fee + ProfitFrom(prices, fee, day + 1, false);
            var hold = ProfitFrom(prices, fee, day + 1, true);
            return Math.Max(sell, hold);
        }

        var buy = -prices[day] + ProfitFrom(prices, fee, day + 1, true);
        var rest = ProfitFrom(prices, fee, day + 1, false);
        return Math.Max(buy, rest);
    }

    // This repo's own Memoizer<TState, TResult> caches that same (day, holding)
    // pair, turning the exponential recursion above into linear work.
    public static int MaxProfitByMemoizedRecursion(int[] prices, int fee)
    {
        return Memoizer.Memoize<(int Day, bool Holding), int>((0, false), ProfitFromMemoized);

        int ProfitFromMemoized((int Day, bool Holding) state, Func<(int Day, bool Holding), int> profit)
        {
            var (day, holding) = state;
            if (day >= prices.Length)
            {
                return 0;
            }

            if (holding)
            {
                var sell = prices[day] - fee + profit((day + 1, false));
                var hold = profit((day + 1, true));
                return Math.Max(sell, hold);
            }

            var buy = -prices[day] + profit((day + 1, true));
            var rest = profit((day + 1, false));
            return Math.Max(buy, rest);
        }
    }
}
