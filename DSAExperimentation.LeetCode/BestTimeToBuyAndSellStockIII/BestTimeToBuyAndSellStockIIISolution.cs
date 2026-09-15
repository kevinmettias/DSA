using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockIII;

// LeetCode 123. Best Time to Buy and Sell Stock III: at most two transactions,
// never two open at once. State is (day, holding, transactions completed so far);
// a transaction only counts as completed on sell, so the cap "at most two" is
// enforced by returning 0 the moment the completed count reaches 2 - no further
// buy is ever explored past that point.
internal static class BestTimeToBuyAndSellStockIIISolution
{
    // Unmemoized recursion over the exact same state machine the composed
    // strategy below runs through Memoizer - up to 2 branches per day, genuinely
    // exponential, the arm the memoized strategy has to beat.
    public static int MaxProfitByBruteForce(int[] prices) => ProfitFrom((0, 0, 0), prices);

    // Same recurrence, run through Algorithms.DynamicProgramming.Memoizer so each
    // of the at-most 2 * n * 3 distinct (day, holding, transactions) states is
    // solved once instead of recomputed down every path that reaches it.
    public static int MaxProfitByTransactionMemoization(int[] prices) =>
        Memoizer.Memoize<(int Day, int Holding, int Transactions), int>(
            (0, 0, 0),
            new ProfitDayByDay(prices));

    private static int ProfitFrom((int Day, int Holding, int Transactions) state, int[] prices)
    {
        if (state.Day == prices.Length || state.Transactions == 2)
        {
            return 0;
        }

        var skip = ProfitFrom((state.Day + 1, state.Holding, state.Transactions), prices);

        return state.Holding == 1
            ? Math.Max(skip, prices[state.Day] + ProfitFrom((state.Day + 1, 0, state.Transactions + 1), prices))
            : Math.Max(skip, -prices[state.Day] + ProfitFrom((state.Day + 1, 1, state.Transactions), prices));
    }

    // The recurrence, named: one day of the state machine, where skipping the day is
    // always allowed and whether a share is held decides what else is. `prices` is the
    // whole of what the rule needs from its caller, so it is the constructor's only
    // input.
    private sealed class ProfitDayByDay(int[] prices)
        : IRecurrence<(int Day, int Holding, int Transactions), int>
    {
        public int Replay(
            (int Day, int Holding, int Transactions) state,
            IRecurrence<(int Day, int Holding, int Transactions), int> rest)
        {
            if (state.Day == prices.Length || state.Transactions == 2)
            {
                return 0;
            }

            var skip = rest.Replay((state.Day + 1, state.Holding, state.Transactions), rest);

            if (state.Holding == 1)
            {
                return Math.Max(skip, prices[state.Day] + rest.Replay((state.Day + 1, 0, state.Transactions + 1), rest));
            }

            return Math.Max(skip, -prices[state.Day] + rest.Replay((state.Day + 1, 1, state.Transactions), rest));
        }
    }
}
