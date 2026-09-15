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
        ProfitFrom(prices, fee, 0, PositionState.Flat);

    // This repo's own Memoizer<TState, TResult> caches that same (day, holding)
    // pair, turning the exponential recursion above into linear work.
    public static int MaxProfitByMemoizedRecursion(int[] prices, int fee) =>
        Memoizer.Memoize<(int Day, bool Holding), int>((0, false), new ProfitDayByDay(prices, fee));

    private static int ProfitFrom(int[] prices, int fee, int day, PositionState position)
    {
        if (day >= prices.Length)
        {
            return 0;
        }

        if (position == PositionState.Holding)
        {
            var sell = prices[day] - fee + ProfitFrom(prices, fee, day + 1, PositionState.Flat);
            var hold = ProfitFrom(prices, fee, day + 1, PositionState.Holding);
            return Math.Max(sell, hold);
        }

        var buy = -prices[day] + ProfitFrom(prices, fee, day + 1, PositionState.Holding);
        var rest = ProfitFrom(prices, fee, day + 1, PositionState.Flat);
        return Math.Max(buy, rest);
    }

    // Whether the day opens holding a share or flat: the state the sell-branch
    // (which pays the fee) and the buy-branch split on. Named after the position
    // - Flat/Holding, the same vocabulary the memoized arm's own state tuple and
    // BestTimeToBuyAndSellStockVSolution use - rather than after the flag, so a
    // call site states which arm it takes.
    private enum PositionState
    {
        Flat,
        Holding,
    }

    // The recurrence, named: one day of the state machine, where a held share can be
    // sold (paying the fee once) or kept, and a flat day can buy or stand still. The
    // day's prices and the fee are the whole of what the rule needs from its caller,
    // so they are the constructor's inputs.
    private sealed class ProfitDayByDay(int[] prices, int fee) : IRecurrence<(int Day, bool Holding), int>
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
                var sell = prices[day] - fee + rest.Replay((day + 1, false), rest);
                var hold = rest.Replay((day + 1, true), rest);
                return Math.Max(sell, hold);
            }

            var buy = -prices[day] + rest.Replay((day + 1, true), rest);
            var idle = rest.Replay((day + 1, false), rest);
            return Math.Max(buy, idle);
        }
    }
}
