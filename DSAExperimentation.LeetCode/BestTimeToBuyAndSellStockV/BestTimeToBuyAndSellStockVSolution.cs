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
    public static long MaxProfitByBruteForce(int[] prices, int k) => ProfitFrom(0, 0, Flat, prices, k);

    private static long ProfitFrom(int day, int used, int position, int[] prices, int k)
    {
        if (day == prices.Length)
        {
            return position == Flat ? 0 : Unreachable;
        }

        var best = ProfitFrom(day + 1, used, position, prices, k);

        if (position == Flat && used < k)
        {
            best = Math.Max(best, -prices[day] + ProfitFrom(day + 1, used + 1, Holding, prices, k));
            best = Math.Max(best, prices[day] + ProfitFrom(day + 1, used + 1, ShortSold, prices, k));
        }
        else if (position == Holding)
        {
            best = Math.Max(best, prices[day] + ProfitFrom(day + 1, used, Flat, prices, k));
        }
        else if (position == ShortSold)
        {
            best = Math.Max(best, -prices[day] + ProfitFrom(day + 1, used, Flat, prices, k));
        }

        return best;
    }

    // Same recurrence, run through Algorithms.DynamicProgramming.Memoizer so
    // each of the at-most 3 * n * (k+1) distinct (day, used, position) states
    // is solved once instead of recomputed down every path that reaches it.
    public static long MaxProfitByTransactionMemoization(int[] prices, int k) =>
        Memoizer.Memoize<(int Day, int Used, int Position), long>(
            (0, 0, Flat),
            (state, recurse) => ProfitFrom(state.Day, state.Used, state.Position, prices, k, recurse));

    private static long ProfitFrom(
        int day, int used, int position, int[] prices, int k,
        Func<(int Day, int Used, int Position), long> recurse)
    {
        if (day == prices.Length)
        {
            return position == Flat ? 0 : Unreachable;
        }

        var best = recurse((day + 1, used, position));

        if (position == Flat && used < k)
        {
            best = Math.Max(best, -prices[day] + recurse((day + 1, used + 1, Holding)));
            best = Math.Max(best, prices[day] + recurse((day + 1, used + 1, ShortSold)));
        }
        else if (position == Holding)
        {
            best = Math.Max(best, prices[day] + recurse((day + 1, used, Flat)));
        }
        else if (position == ShortSold)
        {
            best = Math.Max(best, -prices[day] + recurse((day + 1, used, Flat)));
        }

        return best;
    }
}
