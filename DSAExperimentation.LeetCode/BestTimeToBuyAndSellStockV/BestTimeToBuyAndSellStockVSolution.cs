using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockV;

// LeetCode 3573. Best Time to Buy and Sell Stock V: at most k transactions,
// each either a normal buy-then-sell or a short sell-then-buy-back, never two
// open at once. Buying subtracts the day's price and selling adds it (the
// classic stock-DP fold), so a transaction's profit is just the sum of its two
// half-steps and neither strategy has to remember an entry price - state is
// only (day, transactions started, position: flat/long/short).
internal static class BestTimeToBuyAndSellStockVSolution
{
    private const int Flat = 0;
    private const int Holding = 1;
    private const int ShortSold = 2;

    // A path that reaches day n still holding a position never closed its
    // transaction, so it can't be part of the answer - pushed far enough below
    // any real profit (prices[i] <= 1e9, at most 1000 days) that even 1000
    // stacked half-step adjustments off it can never wrap back around to a
    // positive long.
    private const long Unreachable = long.MinValue / 4;

    // Unmemoized recursion over the exact same state machine the composed
    // strategy below runs through Memoizer - up to 3 branches per day,
    // genuinely exponential, the arm the memoized strategy has to beat.
    public static long MaxProfitByBruteForce(int[] prices, int k) => ProfitFrom((0, 0, Flat), prices, k);

    // Same recurrence, run through Algorithms.DynamicProgramming.Memoizer so
    // each of the at-most 3 * n * (k+1) distinct (day, used, position) states
    // is solved once instead of recomputed down every path that reaches it.
    public static long MaxProfitByTransactionMemoization(int[] prices, int k) =>
        Memoizer.Memoize<(int Day, int Used, int Position), long>(
            (0, 0, Flat),
            new ProfitDayByDay(prices, k));

    private static long ProfitFrom((int Day, int Used, int Position) state, int[] prices, int k)
    {
        if (state.Day == prices.Length)
        {
            return state.Position == Flat ? 0 : Unreachable;
        }

        return BestAfterToday(state, prices, k);
    }

    // One day of the state machine, from a day that still has prices left: doing
    // nothing costs nothing and moves to the next day unchanged, and whatever the
    // open position allows is then tried against that.
    private static long BestAfterToday(
        (int Day, int Used, int Position) state, int[] prices, int transactionBudget)
    {
        var best = ProfitFrom((state.Day + 1, state.Used, state.Position), prices, transactionBudget);

        if (state.Position == Flat && state.Used < transactionBudget)
        {
            best = Math.Max(
                best, -prices[state.Day] + ProfitFrom((state.Day + 1, state.Used + 1, Holding), prices, transactionBudget));
            best = Math.Max(
                best, prices[state.Day] + ProfitFrom((state.Day + 1, state.Used + 1, ShortSold), prices, transactionBudget));
        }
        else if (state.Position == Holding)
        {
            best = Math.Max(
                best, prices[state.Day] + ProfitFrom((state.Day + 1, state.Used, Flat), prices, transactionBudget));
        }
        else if (state.Position == ShortSold)
        {
            best = Math.Max(
                best, -prices[state.Day] + ProfitFrom((state.Day + 1, state.Used, Flat), prices, transactionBudget));
        }

        return best;
    }

    // The recurrence, named: one day of the state machine, from a day that still has
    // prices left - doing nothing costs nothing and moves to the next day unchanged,
    // and whatever the open position allows is then tried against that. The day's
    // prices and the transaction budget are the whole of what the rule needs from its
    // caller, so they are the constructor's inputs.
    private sealed class ProfitDayByDay(int[] prices, int k)
        : IRecurrence<(int Day, int Used, int Position), long>
    {
        public long Replay(
            (int Day, int Used, int Position) state,
            IRecurrence<(int Day, int Used, int Position), long> rest)
        {
            if (state.Day == prices.Length)
            {
                return state.Position == Flat ? 0 : Unreachable;
            }

            var best = rest.Replay((state.Day + 1, state.Used, state.Position), rest);

            if (state.Position == Flat && state.Used < k)
            {
                best = Math.Max(best, -prices[state.Day] + rest.Replay((state.Day + 1, state.Used + 1, Holding), rest));
                best = Math.Max(best, prices[state.Day] + rest.Replay((state.Day + 1, state.Used + 1, ShortSold), rest));
            }
            else if (state.Position == Holding)
            {
                best = Math.Max(best, prices[state.Day] + rest.Replay((state.Day + 1, state.Used, Flat), rest));
            }
            else if (state.Position == ShortSold)
            {
                best = Math.Max(best, -prices[state.Day] + rest.Replay((state.Day + 1, state.Used, Flat), rest));
            }

            return best;
        }
    }
}
