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
    public static int MaxProfitByBruteForce(int[] prices) => ProfitFrom(0, holding: 0, transactions: 0, prices);

    private static int ProfitFrom(int day, int holding, int transactions, int[] prices)
    {
        if (day == prices.Length || transactions == 2)
        {
            return 0;
        }

        var skip = ProfitFrom(day + 1, holding, transactions, prices);

        return holding == 1
            ? Math.Max(skip, prices[day] + ProfitFrom(day + 1, holding: 0, transactions + 1, prices))
            : Math.Max(skip, -prices[day] + ProfitFrom(day + 1, holding: 1, transactions, prices));
    }

    // Same recurrence, run through Algorithms.DynamicProgramming.Memoizer so each
    // of the at-most 2 * n * 3 distinct (day, holding, transactions) states is
    // solved once instead of recomputed down every path that reaches it.
    public static int MaxProfitByTransactionMemoization(int[] prices) =>
        Memoizer.Memoize<(int Day, int Holding, int Transactions), int>(
            (0, 0, 0),
            (state, recurse) => ProfitFrom(state.Day, state.Holding, state.Transactions, prices, recurse));

    private static int ProfitFrom(
        int day, int holding, int transactions, int[] prices,
        Func<(int Day, int Holding, int Transactions), int> recurse)
    {
        if (day == prices.Length || transactions == 2)
        {
            return 0;
        }

        var skip = recurse((day + 1, holding, transactions));

        return holding == 1
            ? Math.Max(skip, prices[day] + recurse((day + 1, 0, transactions + 1)))
            : Math.Max(skip, -prices[day] + recurse((day + 1, 1, transactions)));
    }
}
