using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockIV;

namespace DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockIII;

// LeetCode 123. Best Time to Buy and Sell Stock III: at most two transactions,
// never two open at once. That is LC 188 (Best Time to Buy and Sell Stock IV) with
// its transaction budget fixed at 2, so LC 188's class owns both arms and the
// (day, holding, transactions completed) state machine they run (ARCHITECTURE
// 17.3); each arm here calls through with the cap. This problem's own test and
// benchmark still run them.
internal static class BestTimeToBuyAndSellStockIIISolution
{
    // "At most two transactions" is the whole of what this problem adds to LC 188.
    private const int TransactionCap = 2;

    // Unmemoized recursion over the state machine - up to 2 branches per day,
    // genuinely exponential, the arm the memoized strategy has to beat.
    public static int MaxProfitByBruteForce(int[] prices) =>
        BestTimeToBuyAndSellStockIVSolution.MaxProfitByBruteForce(prices, TransactionCap);

    // The same recurrence through Algorithms.DynamicProgramming.Memoizer, so each of
    // the at-most 2 * n * 3 distinct states is solved once.
    public static int MaxProfitByTransactionMemoization(int[] prices) =>
        BestTimeToBuyAndSellStockIVSolution.MaxProfitByTransactionMemoization(prices, TransactionCap);
}
