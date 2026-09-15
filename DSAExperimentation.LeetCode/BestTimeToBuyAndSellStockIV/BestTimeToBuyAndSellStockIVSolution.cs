using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockIV;

// LeetCode 188. Best Time to Buy and Sell Stock IV: at most k transactions,
// never two open at once. State is (day, holding, transactions completed so
// far); a transaction only counts as completed on sell, so the cap is
// enforced by returning 0 the moment the completed count reaches k - no
// further buy is ever explored past that point. Same state machine as LC 123
// (Best Time to Buy and Sell Stock III), generalized from a fixed k = 2 to
// an arbitrary k.
internal static class BestTimeToBuyAndSellStockIVSolution
{
    // Unmemoized recursion over the exact same state machine the composed
    // strategy below runs through Memoizer - up to 2 branches per day,
    // genuinely exponential, the arm the memoized strategy has to beat.
    public static int MaxProfitByBruteForce(int[] prices, int k) => ProfitFrom((0, 0, 0), prices, k);

    // Same recurrence, run through Algorithms.DynamicProgramming.Memoizer so
    // each of the at-most 2 * n * (k + 1) distinct (day, holding, done)
    // states is solved once instead of recomputed down every path that
    // reaches it.
    public static int MaxProfitByTransactionMemoization(int[] prices, int k) =>
        Memoizer.Memoize<(int Day, int Holding, int Done), int>(
            (0, 0, 0),
            new ProfitDayByDay(prices, k));

    private static int ProfitFrom((int Day, int Holding, int Done) state, int[] prices, int k)
    {
        var (day, holding, done) = state;

        if (day == prices.Length || done == k)
        {
            return 0;
        }

        var skip = ProfitFrom((day + 1, holding, done), prices, k);

        return holding == 1
            ? Math.Max(skip, prices[day] + ProfitFrom((day + 1, 0, done + 1), prices, k))
            : Math.Max(skip, -prices[day] + ProfitFrom((day + 1, 1, done), prices, k));
    }

    // The recurrence, named: one day of the state machine, where skipping the day is
    // always allowed and whether a share is held decides what else is. The day's prices
    // and the transaction budget are the whole of what the rule needs from its caller,
    // so they are the constructor's inputs.
    private sealed class ProfitDayByDay(int[] prices, int k)
        : IRecurrence<(int Day, int Holding, int Done), int>
    {
        public int Replay(
            (int Day, int Holding, int Done) state,
            IRecurrence<(int Day, int Holding, int Done), int> rest)
        {
            var (day, holding, done) = state;

            if (day == prices.Length || done == k)
            {
                return 0;
            }

            var skip = rest.Replay((day + 1, holding, done), rest);

            if (holding == 1)
            {
                return Math.Max(skip, prices[day] + rest.Replay((day + 1, 0, done + 1), rest));
            }

            return Math.Max(skip, -prices[day] + rest.Replay((day + 1, 1, done), rest));
        }
    }
}
