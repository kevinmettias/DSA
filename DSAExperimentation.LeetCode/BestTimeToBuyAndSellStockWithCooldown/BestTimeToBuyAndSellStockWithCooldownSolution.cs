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

    public static int MaxProfitByUnmemoizedRecursion(int[] prices) => ProfitFrom(prices, 0, HoldingState.Flat);

    public static int MaxProfitByMemoizedRecursion(int[] prices) =>
        Memoizer.Memoize<(int Day, bool Holding), int>((0, false), new ProfitDayByDay(prices));

    private static int ProfitFrom(int[] prices, int day, HoldingState holding)
    {
        if (day >= prices.Length)
        {
            return 0;
        }

        if (holding == HoldingState.Holding)
        {
            var sell = prices[day] + ProfitFrom(prices, day + CooldownDays, HoldingState.Flat);
            var hold = ProfitFrom(prices, day + 1, HoldingState.Holding);
            return Math.Max(sell, hold);
        }

        var buy = -prices[day] + ProfitFrom(prices, day + 1, HoldingState.Holding);
        var rest = ProfitFrom(prices, day + 1, HoldingState.Flat);
        return Math.Max(buy, rest);
    }

    // Whether the recurrence is standing on a day with a share already bought, which
    // is the state that decides whether the day's price is a sell or a buy - named so
    // the call site says which one it is rather than spelling it `true`.
    private enum HoldingState
    {
        // A share is held, so this day can sell it (or keep holding).
        Holding,

        // No share is held, so this day can buy one (or rest).
        Flat,
    }

    // The recurrence, named: one day of the state machine, where a held share can be
    // sold (skipping the next day) or kept, and a flat day can buy or stand still.
    // The day's prices are the whole of what the rule needs from its caller, so they
    // are the constructor's only input.
    private sealed class ProfitDayByDay(int[] prices) : IRecurrence<(int Day, bool Holding), int>
    {
        public int Replay((int Day, bool Holding) state, IRecurrence<(int Day, bool Holding), int> rest)
        {
            var (day, holding) = state;
            if (day >= prices.Length)
            {
                return 0;
            }

            if (holding)
            {
                var sell = prices[day] + rest.Replay((day + CooldownDays, false), rest);
                var hold = rest.Replay((day + 1, true), rest);
                return Math.Max(sell, hold);
            }

            var buy = -prices[day] + rest.Replay((day + 1, true), rest);
            var idle = rest.Replay((day + 1, false), rest);
            return Math.Max(buy, idle);
        }
    }
}
