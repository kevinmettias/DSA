using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockWithCooldown;

// LeetCode 309. Best Time to Buy and Sell Stock with Cooldown: state-machine DP over
// (day, holding) - after a sell the very next buy is skipped by advancing two days
// instead of one, which is exactly the cooldown rule.
//
// MaxProfitByMemoizedRecursion composes this repo's own Memoizer<TState,TResult> as
// the cache, keyed by that pair, the same "recurrence takes a memoized recursive
// callback" shape EditDistanceBenchmarks/DecodeWaysTests already use for a 2-tuple
// state.
//
// MaxProfitByUnmemoizedRecursion is the same recurrence with no cache at all - the
// baseline the memoized strategy has to justify itself against, since the same
// (day, holding) state recurs along many different buy/sell/rest paths and the
// naive version re-derives it from scratch every time. Deliberately written without
// this repo's primitives.
internal static class BestTimeToBuyAndSellStockWithCooldownSolution
{
    private const int CooldownDays = 2;

    public static int MaxProfitByUnmemoizedRecursion(int[] prices) => ProfitFrom(prices, 0, false);

    private static int ProfitFrom(int[] prices, int day, bool holding)
    {
        if (day >= prices.Length)
        {
            return 0;
        }

        if (holding)
        {
            var sell = prices[day] + ProfitFrom(prices, day + CooldownDays, false);
            var hold = ProfitFrom(prices, day + 1, true);
            return Math.Max(sell, hold);
        }

        var buy = -prices[day] + ProfitFrom(prices, day + 1, true);
        var rest = ProfitFrom(prices, day + 1, false);
        return Math.Max(buy, rest);
    }

    public static int MaxProfitByMemoizedRecursion(int[] prices)
    {
        return Memoizer.Memoize<(int Day, bool Holding), int>((0, false), ProfitFrom);

        int ProfitFrom((int Day, bool Holding) state, Func<(int Day, bool Holding), int> profit)
        {
            var (day, holding) = state;
            if (day >= prices.Length)
            {
                return 0;
            }

            if (holding)
            {
                var sell = prices[day] + profit((day + CooldownDays, false));
                var hold = profit((day + 1, true));
                return Math.Max(sell, hold);
            }

            var buy = -prices[day] + profit((day + 1, true));
            var rest = profit((day + 1, false));
            return Math.Max(buy, rest);
        }
    }
}
